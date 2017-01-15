using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Common;

namespace Agent.Interfaces
{
public interface IAgent : IActor
{
    Task<ChessPieceInfo> GetInfoAsync();

    Task SetInfoAsync(ChessPieceInfo info);

    Task MoveAsync(int xDirection, int yDirection);
}
}
