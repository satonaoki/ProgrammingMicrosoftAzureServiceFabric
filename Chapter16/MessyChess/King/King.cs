using King.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace King
{
    [ActorService(Name = "King")]
    internal class King : ChessPiece.ChessPiece, IKing
    {
    }
}

