using ChessPiece.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ChessPiece
{
    [ActorService(Name = "ChessPiece")]
    public abstract class ChessPiece : Agent.Agent, IChessPiece
    {
    }
}
