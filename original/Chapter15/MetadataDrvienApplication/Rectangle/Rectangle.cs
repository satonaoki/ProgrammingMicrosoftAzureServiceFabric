using Rectangle.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rectangle
{
    [ActorService(Name = "Rectangle")]
    internal class Rectangle : Shape.Shape, IRectangle
    {
        public override Task<string> DrawAsync()
        {
            return Task.FromResult("Drawing rectangle");
        }
    }
}
