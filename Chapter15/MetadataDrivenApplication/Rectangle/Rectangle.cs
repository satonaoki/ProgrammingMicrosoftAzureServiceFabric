using Rectangle.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System.Threading.Tasks;

namespace Rectangle
{
    [ActorService(Name = "Rectangle")]
    internal class Rectangle : Shape.Shape, IRectangle
    {
        public Rectangle(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public override Task<string> DrawAsync()
        {
            return Task.FromResult("Drawing rectangle");
        }
    }
}
