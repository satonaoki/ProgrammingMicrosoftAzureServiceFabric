using Chessboard.Interfaces;
using Common;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessyChess.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            IChessboard board = ServiceProxy.Create<IChessboard>("3", new Uri("fabric:/MessyChessApplication/Chessboard"));
            board.PutAPieceAsync(new ChessPieceInfo { ActorId = "1", PieceType = ChessPieceType.King, Team = 1, X = 1, Y = 1 }).Wait();
        }
    }
}
