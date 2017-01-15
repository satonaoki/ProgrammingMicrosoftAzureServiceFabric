using Player.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Common;
using ChessPiece.Interfaces;

namespace Player
{

    internal class Player : StatefulActor<Player.ActorState>, IPlayer
    {


        [DataContract]
        internal sealed class ActorState
        {
            [DataMember]
            public ChessPieceInfo ChessPiece { get; set; }
        }

        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                // This is the first time this actor has ever been activated.
                // Set the actor's initial state values.
                this.State = new ActorState { ChessPiece = new ChessPieceInfo { PieceType = ChessPieceType.Undefined } };
            }

            return Task.FromResult(true);
        }


        [Readonly]
        Task<ChessPieceInfo> IPlayer.GetChessPieceInfoAsync()
        {
            return Task.FromResult(this.State.ChessPiece);
        }

        IChessPiece mPieceProxy;
        Task IPlayer.SetChessPieceInfoAsync(ChessPieceInfo piece)
        {
            this.State.ChessPiece.CopyFrom(piece);
            mPieceProxy = ActorProxy.Create<IChessPiece>(new ActorId(piece.ActorId), GetActorUri(piece));
            return Task.FromResult(true);
        }
        async Task IPlayer.MoveAsync(int xDirection, int yDirection)
        {
            if (this.State.ChessPiece.PieceType != ChessPieceType.Undefined && this.State.ChessPiece.ActorId != "")
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
