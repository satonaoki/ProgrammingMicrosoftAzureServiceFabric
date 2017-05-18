using Shape.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shape
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IShape interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by Shape objects.
    /// </remarks>
    
public abstract class Shape : StatelessActor, IShape
{
    public abstract Task<string> DrawAsync();
}
}
