using ActorSwarm.Interfaces;
using Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActorSwarm
{
    [ActorService(Name = "SpatialSwarm")]
    [StatePersistence(StatePersistence.Persisted)]
    internal class ActorSwarm : Actor, IActorSwarm
    {
        int mSize = 100;
        static Random mRand = new Random();

        public ActorSwarm(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            this.StateManager.AddStateAsync<SwarmState>("SwarmState", new SwarmState
            {
                SharedState = new Shared2DArray<byte>(),
                VirtualActorStates = new List<ResidentState>(),
                VirutalActors = new List<IVirtualActor>()
            });

            return Task.FromResult(true);
        }

        public async Task InitializeAsync(int size, float probability)
        {
           var mSwarmState = await this.StateManager.GetStateAsync<SwarmState>("SwarmState");
           mSwarmState.SharedState.Initialize(mSize);
           int count = (int)(size * size * probability);

           for (int i = 0; i < count; i++)
           {
                 mSwarmState.VirtualActorStates.Add(new ResidentState
                 {
                     X = mRand.Next(0, size), Y = mRand.Next(0, size), Tag = (byte)mRand.Next(1, 3)
                 });
                 mSwarmState.VirutalActors.Add(new Resident(size, i, mSwarmState.VirtualActorStates[i], mSwarmState.SharedState));
           }
           await this.StateManager.SetStateAsync<SwarmState>("SwarmState", mSwarmState);
        }

        public async Task<string> ReadStateStringAsync()
        {
            var mSwarmState = await this.StateManager.GetStateAsync<SwarmState>("SwarmState");
            return mSwarmState.SharedState.ToString();
        }

        public async Task EvolveAsync()
        {
            var mSwarmState = await this.StateManager.GetStateAsync<SwarmState>("SwarmState");
            foreach (var actor in mSwarmState.VirutalActors)
            {
                mSwarmState.SharedState.Propose(await actor.ProposeAsync());
            }
            mSwarmState.SharedState.ResolveConflictsAndCommit((p) => {
                if (p is Proposal2D<byte>)
                {
                    var proposal = (Proposal2D<byte>)p;
                    mSwarmState.VirutalActors[proposal.ActorId].ApproveProposalAsync(proposal);
                }
            });
            await this.StateManager.SetStateAsync<SwarmState>("SwarmState", mSwarmState);
        }
    }
}
