using ChessboardObserver.Interfaces;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChessboardObserver
{
    internal sealed class ChessboardObserver : StatefulService, IChessboardObserver
    {
        private const string mDictionaryName = "board";
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(parameters => new ServiceRemotingListener<ChessboardObserver>(parameters, this)) };
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
        public async Task<KeyValuePair<string, List<ChessPieceInfo>>[]> GetBoard()
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ChessPieceInfo>>>(mDictionaryName);
            return myDictionary.ToArray();
        }
    }
}
