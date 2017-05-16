using ChessPiece.Interfaces;
using Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using Player.Interfaces;
using System;
using System.Threading.Tasks;

namespace Player
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Player : Actor, IPlayer
    {
        public Player(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override async Task OnActivateAsync()
        {
            var result = await this.StateManager.TryGetStateAsync<ChessPieceInfo>("ChessPiece");

            if (!result.HasValue)
            {
                await this.StateManager.SetStateAsync<ChessPieceInfo>("ChessPiece", new ChessPieceInfo { PieceType = ChessPieceType.Undefined });
            }
        }

        Task<ChessPieceInfo> IPlayer.GetChessPieceInfoAsync()
        {
            return this.StateManager.GetStateAsync<ChessPieceInfo>("ChessPiece");
        }

        IChessPiece mPieceProxy;

        Task IPlayer.SetChessPieceInfoAsync(ChessPieceInfo piece)
        {
            this.StateManager.SetStateAsync<ChessPieceInfo>("ChessPiece", piece);
            mPieceProxy = ActorProxy.Create<IChessPiece>(new ActorId(piece.ActorId), GetActorUri(piece));
            return Task.FromResult(true);
        }
        async Task IPlayer.MoveAsync(int xDirection, int yDirection)
        {
            var piece = await this.StateManager.GetStateAsync<ChessPieceInfo>("ChessPiece");

            if (piece.PieceType != ChessPieceType.Undefined && piece.ActorId != "")
            {
                await mPieceProxy.MoveAsync(xDirection, yDirection);
            }
        }
        Uri GetActorUri(ChessPieceInfo piece)
        {
            throw new NotImplementedException();
        }
    }
}
