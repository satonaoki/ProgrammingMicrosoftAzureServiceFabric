using Player.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace Player
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Player : Actor, IPlayer
    {
        public Player(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            var result = await this.StateManager.TryGetStateAsync<int>("Count");
            if (!result.HasValue)
            {
                await this.StateManager.SetStateAsync<int>("Count", 0);
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.StateManager.GetStateAsync<int>("Count"));
        }

        Task<int> IPlayer.GetCountAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Getting current count value as {0}", this.StateManager.GetStateAsync<int>("Count"));
            return this.StateManager.GetStateAsync<int>("Count");
        }

        Task IPlayer.SetCountAsync(int count)
        {
            ActorEventSource.Current.ActorMessage(this, "Setting current count of value to {0}", count);

            return this.StateManager.SetStateAsync<int>("Count", count);
        }
    }
}
