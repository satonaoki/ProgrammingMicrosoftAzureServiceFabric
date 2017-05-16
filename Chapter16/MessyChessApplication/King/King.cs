using King.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace King
{
    [ActorService(Name = "King")]
    public class King : ChessPiece.ChessPiece, IKing
    {
        public King(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }
    }
}

