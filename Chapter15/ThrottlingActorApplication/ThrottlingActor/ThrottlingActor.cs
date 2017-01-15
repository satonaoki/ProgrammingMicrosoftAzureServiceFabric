using ThrottlingActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ThrottlingActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
[ActorService(Name = "ThrottlingActor")]
internal class ThrottlingActor : StatefulActor<ThrottlingActor.ActorState>, IThrottlingActor, IThrottlingActorManagement
    {
        [DataContract]
        internal sealed class ActorState
        {
            [DataMember]
            public int Credit { get; set; }
            [DataMember]
            public Dictionary<string, int> ServiceCreditScore { get; set; }
        }

        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                // This is the first time this actor has ever been activated.
                // Set the actor's initial state values.
                this.State = new ActorState
                {
                    Credit = 100,
                    ServiceCreditScore = new Dictionary<string, int>
                    {
                        {"action1", 50 },
                        {"action2", 25 },
                        {"action3", 10 }
                    }
                };
            }

            return Task.FromResult(true);
        }

Task<int> IThrottlingActorManagement.AddCreditsAsync(int credits)
{
    this.State.Credit += credits;
    return Task.FromResult(this.State.Credit);
}

        Task<string> IThrottlingActor.GetAccessTokenAsync(string actionId)
        {
            if (!this.State.ServiceCreditScore.ContainsKey(actionId))
                return Task.FromResult("Unsupported action.");
            int credit = this.State.ServiceCreditScore[actionId];
            if (this.State.Credit - credit >= 0)
            {
                this.State.Credit -= credit;
                string token = "Remaining credit: " + this.State.Credit;
                //1. Generate a symetric/asemetric key
                //2. Generate an access token
                //3. Encrypt and optionally sign the token
                return Task.FromResult(token);
            }
            else
                return Task.FromResult("Insufficient credit");
        }
    }
}
