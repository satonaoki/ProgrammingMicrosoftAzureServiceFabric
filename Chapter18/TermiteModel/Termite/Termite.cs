using Termite.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Box.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Termite
{
    internal class Termite : StatefulActor<TermiteState>, ITermite
    {
        private IActorTimer mTimer;
        private static Random rand = new Random();
        private const int size = 100;
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                this.State = new TermiteState { X = rand.Next(0, size), Y = rand.Next(0, size), HasWoodchip = false };
            }
            mTimer = RegisterTimer(Move, this.State, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(50));
            return Task.FromResult(true);
        }
        protected override Task OnDeactivateAsync()
        {
            if (mTimer != null)
                UnregisterTimer(mTimer);
            return base.OnDeactivateAsync();
        }
        [Readonly]
        Task<TermiteState> ITermite.GetStateAsync()
        {
            return Task.FromResult(this.State);
        }
        private async Task Move(Object state)
        {
            IBox boxClient = ServiceProxy.Create<IBox>(new Uri("fabric:/TermiteModel/Box"));
            int action = rand.Next(1, 9);
            //1-left; 2-left-up; 3-up; 4-up-right; 5-right: 6-right-down; 7-down; 8-down-left
            if ((action == 1 || action == 2 || action == 8) && this.State.X > 0)
                this.State.X = this.State.X - 1;
            if ((action == 4 || action == 5 || action == 6) && this.State.X < size - 1)
                this.State.X = this.State.X + 1;
            if ((action == 2 || action == 3 || action == 4) && this.State.Y > 0)
                this.State.Y = this.State.Y - 1;
            if ((action == 6 || action == 7 || action == 8) && this.State.Y < size - 1)
                this.State.Y = this.State.Y + 1;
            if (!this.State.HasWoodchip)
            {
                var result = await boxClient.TryPickUpWoodChipAsync(this.State.X, this.State.Y);
                if (result)
                    this.State.HasWoodchip = true;
            }
            else
            {
                var result = await boxClient.TryPutDonwWoodChipAsync(this.State.X, this.State.Y);
                if (result)
                    this.State.HasWoodchip = false;
            }
        }
    }
}
