using Chessboard.Interfaces;
using ChessboardObserver.Interfaces;
using Common;
using King.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessyChess.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest("Test Catalog", () => TestCatalog());
            Console.ReadLine();
        }
        static void RunTest(string title, Action test)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(title);
            Console.ResetColor();
            test();
            Console.WriteLine();
        }
        static void TestCatalog()
        {
            ChessPieceCatalog catalog = ChessPieceCatalog.Initialize();
            List<ChessPieceType> pieces = new List<ChessPieceType>();
            for (int i = 0; i < 20; i++)
            {
                var piece = catalog.GetAPiece();
                pieces.Add(piece);
                Console.WriteLine("Got: " + piece);
            }
            foreach (var piece in pieces)
            {
                if (piece != ChessPieceType.Undefined)
                {
                    Console.WriteLine("Returning " + piece);
                    catalog.ReturnAPiece(piece);
                }
            }
            try
            {
                catalog.ReturnAPiece(ChessPieceType.King);
            }
            catch (ArgumentException exp)
            {
                Console.WriteLine(exp.Message);
            }

        }
        static void TestMisc()
        {
            var proxy = ActorProxy.Create<IKing>(new ActorId("1"), "fabric:/MessyChessApplication", "King");
            proxy.SetInfoAsync(new ChessPieceInfo { ActorId = "1", PieceType = ChessPieceType.King, Team = 1, X = 1, Y = 1 }).Wait();
            var king = proxy.GetInfoAsync().Result;
            Console.WriteLine(king.PieceType);

            Console.ReadLine();

            Random rand = new Random();
            while (true)
            {
                int x = 0;
                int y = 0;
                while (x < 8 && y < 8 || x >= 16 && y < 8
                        || x < 8 && y >= 16 || x >= 16 && y >= 16)
                {
                    x = rand.Next(0, 24);
                    y = rand.Next(0, 24);
                }
                string partition = ((y / 4) * 6 + (x / 4) + 1).ToString();
                IChessboard boardShard = ServiceProxy.Create<IChessboard>(partition, new Uri("fabric:/MessyChessApplication/Chessboard"));
                boardShard.PutAPieceAsync(new ChessPieceInfo { ActorId = "1", PieceType = ChessPieceType.King, Team = rand.Next(1, 5), X = x, Y = y }).Wait();
                IChessboardObserver board = ServiceProxy.Create<IChessboardObserver>(new Uri("fabric:/MessyChessApplication/ChessboardObserver"));
                var boardInfo = board.GetBoard().Result;
                PaintBoard(boardInfo);
                Thread.Sleep(10);
            }
        }
        static void PaintBoard(KeyValuePair<string, List<ChessPieceInfo>>[] boardInfo)
        {
            bool flip = true;
            for (int y = 0; y < 24; y++)
            {
                for (int x =0; x < 24; x++)
                {
                    Console.CursorLeft = x;
                    Console.CursorTop = y;
                    if (x < 8 && y < 8 || x >= 16 && y < 8
                        || x < 8 && y >= 16 || x >= 16 && y >= 16)
                        Console.Write(" ");
                    else {
                        Console.BackgroundColor = flip ? ConsoleColor.White : ConsoleColor.Gray;
                        Console.Write(" ");
                        Console.ResetColor();
                        flip = !flip;
                    }
                }
                flip = !flip;
                Console.WriteLine();
            }
            foreach (var kv in boardInfo)
            {
                foreach (var v in kv.Value)
                {
                    Console.CursorLeft = v.X;
                    Console.CursorTop = v.Y;
                    Console.BackgroundColor = (v.X % 2 == 0 && v.Y % 2 == 0
                        || v.X % 2 == 1 && v.Y % 2 == 1) ? ConsoleColor.White : ConsoleColor.Gray;
                    switch (v.Team)
                    {
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                    }
                    Console.Write(v.PieceType.ToString()[0]);
                    Console.ResetColor();
                }
            }
        }
    }
}
