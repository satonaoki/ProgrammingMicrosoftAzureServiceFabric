using Box.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Threading.Tasks;
using Termite.Interfaces;

namespace Termite
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Termite : Actor, ITermite
    {
        private IActorTimer mTimer;
        private static Random rand = new Random();
        private const int size = 100;
        private IBox boxClient;

        public Termite(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            this.StateManager.AddStateAsync<TermiteState>("State", new TermiteState { X = rand.Next(0, size), Y = rand.Next(0, size), HasWoodchip = false });
            mTimer = RegisterTimer(Move, this.StateManager.GetStateAsync<TermiteState>("State"), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(50));
            boxClient = ServiceProxy.Create<IBox>(new Uri("fabric:/TermiteModel/Box"), new ServicePartitionKey(0));
            return Task.FromResult(true);
        }

        protected override Task OnDeactivateAsync()
        {
            if (mTimer != null)
                UnregisterTimer(mTimer);
            return base.OnDeactivateAsync();
        }

        public Task<TermiteState> GetStateAsync()
        {
            return this.StateManager.GetStateAsync<TermiteState>("State");
        }

        private async Task Move(Object state)
        {
            //IBox boxClient = ServiceProxy.Create<IBox>(new Uri("fabric:/TermiteModel/Box"), new ServicePartitionKey(0));
 
            TermiteState mState = await this.StateManager.GetStateAsync<TermiteState>("State");
            
            if (!mState.HasWoodchip)
            {
                var result = await boxClient.TryPickUpWoodChipAsync(mState.X, mState.Y);
                if (result)
                    mState.HasWoodchip = true;
            }
            else
            {
                var result = await boxClient.TryPutDonwWoodChipAsync(mState.X, mState.Y);
                if (result)
                    mState.HasWoodchip = false;
            }
            await this.StateManager.SetStateAsync<TermiteState>("State", mState);

            int action = rand.Next(1, 9);
            //1-left; 2-left-up; 3-up; 4-up-right; 5-right: 6-right-down; 7-down; 8-down-left
            if ((action == 1 || action == 2 || action == 8) && mState.X > 0)
                mState.X = mState.X - 1;
            if ((action == 4 || action == 5 || action == 6) && mState.X < size - 1)
                mState.X = mState.X + 1;
            if ((action == 2 || action == 3 || action == 4) && mState.Y > 0)
                mState.Y = mState.Y - 1;
            if ((action == 6 || action == 7 || action == 8) && mState.Y < size - 1)
                mState.Y = mState.Y + 1;
        }
    }
}
