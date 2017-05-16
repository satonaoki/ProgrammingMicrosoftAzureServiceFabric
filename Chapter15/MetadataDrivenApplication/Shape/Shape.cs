using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Shape.Interfaces;
using System.Threading.Tasks;

namespace Shape
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IShape interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by Shape objects.
    /// </remarks>

    public abstract class Shape : Actor, IShape
    {
        public Shape(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public abstract Task<string> DrawAsync();
    }
}
