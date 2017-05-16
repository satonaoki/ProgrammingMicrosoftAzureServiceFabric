using Common;
using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace Agent.Interfaces
{
    public interface IAgent : IActor
    {
        Task<ChessPieceInfo> GetInfoAsync();

        Task SetInfoAsync(ChessPieceInfo info);

        Task MoveAsync(int xDirection, int yDirection);
    }
}
