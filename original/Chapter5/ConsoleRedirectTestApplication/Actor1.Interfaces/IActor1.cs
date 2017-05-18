using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Actor1.Interfaces
{
    public interface IActor1 : IActor
    {
        Task<string> DoWorkAsync();
    }
}
