using Agent.Interfaces;
using Common;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Agent
{
    public abstract class Agent : StatefulActor<Agent.ActorState>, IAgent
    {
        [DataContract]
        public sealed class ActorState
        {
            [DataMember]
            public ChessPieceInfo Info { get; set; }
        }
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                this.State = new ActorState { Info = new ChessPieceInfo()}; 
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }


        [Readonly]
        Task<ChessPieceInfo> IAgent.GetInfoAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Getting current count value as {0}", this.State.Info);
            return Task.FromResult(this.State.Info);
        }

        Task IAgent.SetInfoAsync(ChessPieceInfo info)
        {
            ActorEventSource.Current.ActorMessage(this, "Setting current count of value to {0}", info);
            this.State.Info.CopyFrom(info);

            return Task.FromResult(true);
        }
        Task IAgent.MoveAsync(int xDirection, int yDirection)
        {
            throw new NotImplementedException();
        }
    }
}
