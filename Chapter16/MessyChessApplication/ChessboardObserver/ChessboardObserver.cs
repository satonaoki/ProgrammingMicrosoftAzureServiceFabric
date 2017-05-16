using ChessboardObserver.Interfaces;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Linq;

namespace ChessboardObserver
{
    /// <summary>
    /// Service Fabric ランタイムによって、このクラスのインスタンスがサービス レプリカごとに作成されます。
    /// </summary>
    internal sealed class ChessboardObserver : StatefulService, IChessboardObserver
    {
        public ChessboardObserver(StatefulServiceContext context)
            : base(context)
        { }

        private const string mDictionaryName = "board";
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) };
        }
        public async Task Notify(string partitionName, List<ChessPieceInfo> boardShard)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ChessPieceInfo>>>(mDictionaryName);
            using (var tx = this.StateManager.CreateTransaction())
            {
                await myDictionary.AddOrUpdateAsync(tx, partitionName, boardShard, (k, v) => boardShard);
                await tx.CommitAsync();
            }
        }
        public async Task<KeyValuePair<string, List<ChessPieceInfo>>[]> GetBoard(CancellationToken ct)
        {
            var results = new List<KeyValuePair<string, List<ChessPieceInfo>>>();

            using (var tx = this.StateManager.CreateTransaction())
            {
                var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ChessPieceInfo>>>(mDictionaryName);

                var enumerator = (await myDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    results.Add(new KeyValuePair<string, List<ChessPieceInfo>>(enumerator.Current.Key, enumerator.Current.Value));
                }

                return results.ToArray();
            }
        }
    }
}
