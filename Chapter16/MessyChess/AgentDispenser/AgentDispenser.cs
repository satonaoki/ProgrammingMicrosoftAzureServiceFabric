using AgentDispenser.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AgentDispenser
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
    internal class AgentDispenser : StatefulActor<AgentDispenser.ActorState>, IAgentDispenser
    {
        /// <summary>
        /// This class contains each actor's replicated state.
        /// Each instance of this class is serialized and replicated every time an actor's state is saved.
        /// For more information, see http://aka.ms/servicefabricactorsstateserialization
        /// </summary>
        [DataContract]
        internal sealed class ActorState
        {
            [DataMember]
            public int Count { get; set; }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "AgentDispenser.ActorState[Count = {0}]", Count);
            }
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                // This is the first time this actor has ever been activated.
                // Set the actor's initial state values.
                this.State = new ActorState { Count = 0 };
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }


        [Readonly]
        Task<int> IAgentDispenser.GetCountAsync()
        {
            // For methods that do not change the actor's state,
            // [Readonly] improves performance by not performing serialization and replication of the actor's state.
            ActorEventSource.Current.ActorMessage(this, "Getting current count value as {0}", this.State.Count);
            return Task.FromResult(this.State.Count);
        }

        Task IAgentDispenser.SetCountAsync(int count)
        {
            ActorEventSource.Current.ActorMessage(this, "Setting current count of value to {0}", count);
            this.State.Count = count;  // Update the state

            return Task.FromResult(true);
            // When this method returns, the Actor framework automatically
            // serializes & replicates the actor's state.
        }
    }
}
