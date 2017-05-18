using Player.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Interfaces;

namespace Player
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IPlayer interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by Player objects.
    /// </remarks>
    internal class Player : StatelessActor, IPlayer
    {
        
        public Task<bool> JoinGameAsync(ActorId gameId, string playerName)
        {
            var game = ActorProxy.Create<IGame>(gameId, "fabric:/ActorTicTacToeApplication");
            return game.JoinGameAsync(this.Id.GetLongId(), playerName);
        }

        public Task<bool> MakeMoveAsync(ActorId gameId, int x, int y)
        {
            var game = ActorProxy.Create<IGame>(gameId, "fabric:/ActorTicTacToeApplication");
            return game.MakeMoveAsync(this.Id.GetLongId(), x, y);
        }
    }
}
