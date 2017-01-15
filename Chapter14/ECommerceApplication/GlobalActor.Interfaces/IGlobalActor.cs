using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace GlobalActor.Interfaces
{
    public interface IGlobalActor : IActor
    {
        Task<List<Tuple<string, long>>> CountGlobalSalesAsync();
    }
}
