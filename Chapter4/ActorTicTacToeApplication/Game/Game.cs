using Game.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    [VolatileActorStateProvider]
    internal class Game : StatefulActor<Game.ActorState>, IGame
    {
        /// <summary>
        /// This class contains each actor's replicated state.
        /// Each instance of this class is serialized and replicated every time an actor's state is saved.
        /// For more information, see http://aka.ms/servicefabricactorsstateserialization
        /// </summary>
        [DataContract]
        public class ActorState

        {
            [DataMember]
            public int[] Board;
            [DataMember]
            public string Winner;
            [DataMember]
            public List<Tuple<long, string>> Players;
            [DataMember]
            public int NextPlayerIndex;
            [DataMember]
            public int NumberOfMoves;
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                this.State = new ActorState()
                {
                    Board = new int[9],
                    Winner = "",
                    Players = new List<Tuple<long, string>>(),
                    NextPlayerIndex = 0,
                    NumberOfMoves = 0
                };
            }
            return Task.FromResult(true);
        }
        public Task<bool> JoinGameAsync(long playerId, string playerName)
        {
            if (this.State.Players.Count >= 2
                || this.State.Players.FirstOrDefault(p => p.Item2 == playerName) != null)
                return Task.FromResult<bool>(false);
            this.State.Players.Add(new Tuple<long, string>(playerId, playerName));
            return Task.FromResult<bool>(true);
        }
        [Readonly]
        public Task<int[]> GetGameBoardAsync()
        {
            return Task.FromResult<int[]>(this.State.Board);
        }
        [Readonly]
        public Task<string> GetWinnerAsync()
        {
            return Task.FromResult<string>(this.State.Winner);
        }
        public Task<bool> MakeMoveAsync(long playerId, int x, int y)
        {
            if (x < 0 || x > 2 || y < 0 || y > 2
                || this.State.Players.Count != 2
                || this.State.NumberOfMoves >= 9
                || this.State.Winner != "")
                return Task.FromResult<bool>(false);

            int index = this.State.Players.FindIndex(p => p.Item1 == playerId);
            if (index == this.State.NextPlayerIndex)
            {
                if (this.State.Board[y * 3 + x] == 0)
                {
                    int piece = index * 2 - 1;
                    this.State.Board[y * 3 + x] = piece;
                    this.State.NumberOfMoves++;

                    if (HasWon(piece * 3))
                        this.State.Winner = this.State.Players[index].Item2 + " (" +
                                           (piece == -1 ? "X" : "O") + ")";
                    else if (this.State.Winner == "" && this.State.NumberOfMoves >= 9)

                        this.State.Winner = "TIE";

                    this.State.NextPlayerIndex = (this.State.NextPlayerIndex + 1) % 2;
                    return Task.FromResult<bool>(true);
                }
                else
                    return Task.FromResult<bool>(false);
            }
            else
                return Task.FromResult<bool>(false);
        }
        private bool HasWon(int sum)
        {
            return this.State.Board[0] + this.State.Board[1] + this.State.Board[2] == sum
                || this.State.Board[3] + this.State.Board[4] + this.State.Board[5] == sum
                || this.State.Board[6] + this.State.Board[7] + this.State.Board[8] == sum
                || this.State.Board[0] + this.State.Board[3] + this.State.Board[6] == sum
                || this.State.Board[1] + this.State.Board[4] + this.State.Board[7] == sum
                || this.State.Board[2] + this.State.Board[5] + this.State.Board[8] == sum
                || this.State.Board[0] + this.State.Board[4] + this.State.Board[8] == sum
                || this.State.Board[2] + this.State.Board[4] + this.State.Board[6] == sum;
        }
    }
}
