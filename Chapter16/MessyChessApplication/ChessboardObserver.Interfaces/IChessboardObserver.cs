using Common;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChessboardObserver.Interfaces
{
    public interface IChessboardObserver : IService
    {
        Task Notify(string partitionName, List<ChessPieceInfo> boardShard);
        Task<KeyValuePair<string, List<ChessPieceInfo>>[]> GetBoard(CancellationToken ct);
    }
}
