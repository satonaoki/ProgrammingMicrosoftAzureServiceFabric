using Actor1.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Actor1
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IActor1 interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by Actor1 objects.
    /// </remarks>
    internal class Actor1 : StatelessActor, IActor1
    {
        Task<string> IActor1.DoWorkAsync()
        {
            // TODO: Replace the following with your own logic.
            ActorEventSource.Current.ActorMessage(this, "Doing Work");

            return Task.FromResult("Work result");
        }
    }
}
