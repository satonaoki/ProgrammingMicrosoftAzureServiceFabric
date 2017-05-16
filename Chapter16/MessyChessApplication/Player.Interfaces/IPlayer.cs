using Common;
using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace Player.Interfaces
{
    /// <summary>
    /// This interface represents the actions a client app can perform on an actor.
    /// It MUST derive from IActor and all methods MUST return a Task.
    /// </summary>
    public interface IPlayer : IActor
    {
        Task<ChessPieceInfo> GetChessPieceInfoAsync();

        Task SetChessPieceInfoAsync(ChessPieceInfo piece);

        Task MoveAsync(int xDirection, int yDirection);
    }
}
