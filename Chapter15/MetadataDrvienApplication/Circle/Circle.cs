using Circle.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Circle
{
    [ActorService(Name = "Circle")]
    internal class Circle : Shape.Shape, ICircle
    {
        public override Task<string> DrawAsync()
        {
            return Task.FromResult("Drawing circle");
        }
    }
}
