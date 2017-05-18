using ProductActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ProductActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
internal class ProductActor : StatefulActor<ProductActor.ActorState>, IProductActor
{
    [DataContract]
    internal sealed class ActorState
    {
        [DataMember]
        public int Sales { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "ProductActor.ActorState[Sales = {0}]", Sales);
        }
    }
    protected override Task OnActivateAsync()
    {
        if (this.State == null)
        {
            this.State = new ActorState { Sales = 0 };
        }

        ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
        return Task.FromResult(true);
    }

    [Readonly]
    Task<int> IProductActor.GetSalesAsync()
    {
        ActorEventSource.Current.ActorMessage(this, "Getting current sales value as {0}", this.State.Sales);
        return Task.FromResult(this.State.Sales);
    }

    Task IProductActor.SellAsync()
    {
            
        this.State.Sales += 1;  
        return Task.FromResult(true);
    }
}
}
