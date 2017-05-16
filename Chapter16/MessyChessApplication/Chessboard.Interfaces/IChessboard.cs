using Common;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Chessboard.Interfaces
{
    public interface IChessboard : IService
    {
        Task TakeAPieceAsync(ChessPieceInfo piece);
        Task PutAPieceAsync(ChessPieceInfo piece);
    }
}
