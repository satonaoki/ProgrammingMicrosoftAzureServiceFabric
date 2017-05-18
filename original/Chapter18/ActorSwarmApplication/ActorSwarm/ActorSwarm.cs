using ActorSwarm.Interfaces;
using ActorSwarm.Common;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ActorSwarm
{
    [ActorService(Name ="SpatialSwarm")]
    internal class ActorSwarm : StatefulActor<SwarmState>, IActorSwarm
    {
        int mSize = 100;
        static Random mRand = new Random();
        
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                this.State = new SwarmState
                {
                    SharedState = new Shared2DArray<byte>(),
                    VirtualActorStates = new List< ResidentState>(),
                    VirutalActors = new List<IVirtualActor>()
                };
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }


        public Task InitializeAsync(int size, float probability)
        {
            this.State.SharedState.Initialize(mSize);
            int count = (int)(size * size * probability);
            for (int i = 0; i < count; i++)
            {
                this.State.VirtualActorStates.Add(new ResidentState { X = mRand.Next(0, size), Y = mRand.Next(0, size), Tag = (byte)mRand.Next(1,3) });
                this.State.VirutalActors.Add(new Resident(size, i, this.State.VirtualActorStates[i], this.State.SharedState));
            }
            return Task.FromResult(1);
        }
        public Task<string> ReadStateStringAsync()
        {
            return Task.FromResult<string>(this.State.SharedState.ToString());
        }

        public async Task EvolveAsync()
        {
            foreach (var actor in this.State.VirutalActors)
            {
                this.State.SharedState.Propose(await actor.ProposeAsync());
            }
            this.State.SharedState.ResolveConflictsAndCommit((p)=> {
                if (p is Proposal2D<byte>)
                {
                    var proposal = (Proposal2D<byte>)p;
                    this.State.VirutalActors[proposal.ActorId].ApproveProposalAsync(proposal);
                }
            });
        }
    }
}
