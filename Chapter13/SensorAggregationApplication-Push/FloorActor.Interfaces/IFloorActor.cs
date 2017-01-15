using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace FloorActor.Interfaces
{
    /// <summary>
    /// This interface represents the actions a client app can perform on an actor.
    /// It MUST derive from IActor and all methods MUST return a Task.
    /// </summary>
public interface IFloorActor : IActor
{
    Task<double> GetTemperatureAsync();
    Task SetTemperatureAsync(int index, double temperature);
}
}
