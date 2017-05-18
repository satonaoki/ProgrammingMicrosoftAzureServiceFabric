using FloorActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using SensorActor.Interfaces;
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
            public double Temperature { get; set; }

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
                this.State = new ActorState { Temperature = 0.0 };
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }


        [Readonly]
        Task<double> IFloorActor.GetTemperatureAsync()
        {
            Task<double>[] tasks = new Task<double>[1000];
            double[] readings = new double[1000];
            Parallel.For(0, 1000, i =>
            {
                var proxy = ActorProxy.Create<ISensorActor>(new ActorId(i), "fabric:/SensorAggregationApplication");
                tasks[i] = proxy.GetTemperatureAsync();
            });
            Task.WaitAll(tasks);
            Parallel.For(0, 1000, i =>
            {
                readings[i] = tasks[i].Result;
            });
            return Task.FromResult(readings.Average());
        }
    }
}
