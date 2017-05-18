using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Shape.Interfaces
{
public interface IShape : IActor
{
    Task<string> DrawAsync();
}
}
