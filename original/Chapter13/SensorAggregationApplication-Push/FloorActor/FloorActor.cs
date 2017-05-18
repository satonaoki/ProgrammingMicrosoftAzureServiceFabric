using FloorActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FloorActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
internal class FloorActor : StatefulActor<FloorActor.ActorState>, IFloorActor
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
        public double[] Temperature { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "FloorActor.ActorState[Temperature = {0}]", Temperature);
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
            this.State = new ActorState { Temperature = new double[1000] };
        }

        ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
        return Task.FromResult(true);
    }


    [Readonly]
    Task<double> IFloorActor.GetTemperatureAsync()
    {
        // For methods that do not change the actor's state,
        // [Readonly] improves performance by not performing serialization and replication of the actor's state.
        ActorEventSource.Current.ActorMessage(this, "Getting current temperature value as {0}", this.State.Temperature);
            
        return Task.FromResult(this.State.Temperature.Average());
    }

    Task IFloorActor.SetTemperatureAsync(int index, double temperature)
    {
        ActorEventSource.Current.ActorMessage(this, "Setting current temperature of value to {0}", temperature);
        this.State.Temperature[index] = temperature;  // Update the state

        return Task.FromResult(true);
        // When this method returns, the Actor framework automatically
        // serializes & replicates the actor's state.
    }
}
}
