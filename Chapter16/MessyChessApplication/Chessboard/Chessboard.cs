using Chessboard.Interfaces;
using ChessboardObserver.Interfaces;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chessboard
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Chessboard : StatefulService, IChessboard
    {
        private List<IChessboardObserver> mObservers = new List<IChessboardObserver>();

        public Chessboard(StatefulServiceContext context)
            : base(context)
        { }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            string serviceName = this.Context.ServiceName.ToString();
            serviceName = serviceName.Substring(0, serviceName.LastIndexOf("/")) + "/ChessboardObserver";
            mObservers.Add(ServiceProxy.Create<IChessboardObserver>(new Uri(serviceName)));
            return Task.FromResult(1);
        }
        private const string mDictionaryName = "boardShard";


        private void ko()
        {

        }
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) };
        }
        private async Task NotifyObservers(CancellationToken ct)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, ChessPieceInfo>>(mDictionaryName);
            var results = new List<ChessPieceInfo>();

            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerator = (await myDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    results.Add(enumerator.Current.Value);
                }

                Parallel.ForEach<IChessboardObserver>(mObservers, observer => observer.Notify(this.Context.PartitionId.ToString("D"), results));
            }
        }
        public async Task TakeAPieceAsync(ChessPieceInfo piece)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, ChessPieceInfo>>(mDictionaryName);
            var key = piece.X + "-" + piece.Y;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await myDictionary.TryGetValueAsync(tx, key);
                if (result.HasValue && result.Value.ActorId == piece.ActorId)
                {
                    await myDictionary.SetAsync(tx, key, ChessPieceInfo.Empty);
                }
                await tx.CommitAsync();
            }
            await NotifyObservers(new CancellationToken());
        }
        public async Task PutAPieceAsync(ChessPieceInfo piece)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, ChessPieceInfo>>(mDictionaryName);
            var key = piece.X + "-" + piece.Y;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await myDictionary.TryGetValueAsync(tx, key);
                if (result.HasValue)
                {
                    if (result.Value.ActorId == "")
                    {
                        await myDictionary.SetAsync(tx, key, piece);
                    }
                    else
                    {
                        //TODO: Take a piece
                    }
                }
                else
                {
                    await myDictionary.AddAsync(tx, key, piece);
                }
                await tx.CommitAsync();
            }
            await NotifyObservers(new CancellationToken());
        }

        private string getDictionaryName(int x, int y)
        {
            return "boardShard" + ((y / 4) * 6 + (x / 4) + 1);
        }
    }
}
