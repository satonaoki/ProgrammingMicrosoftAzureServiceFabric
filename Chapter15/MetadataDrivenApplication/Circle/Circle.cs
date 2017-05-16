using Circle.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System.Threading.Tasks;

namespace Circle
{
    [ActorService(Name = "Circle")]
    internal class Circle : Shape.Shape, ICircle
    {
        public Circle(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public override Task<string> DrawAsync()
        {
            return Task.FromResult("Drawing circle");
        }
    }
}
