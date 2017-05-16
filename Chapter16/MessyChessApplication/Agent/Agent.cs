using Agent.Interfaces;
using Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Threading.Tasks;

namespace Agent
{
    /// <remarks>
    /// このクラスはアクターを表します。
    /// 各 ActorID がこのクラスのインスタンスにマップされます。
    /// StatePersistence 属性はアクターの状態の永続化とレプリケーションを次のように決定します:
    ///  - 永続化: 状態はディスクに書き込まれ、レプリケートされます。
    ///  - 可変: 状態はメモリにのみ保持され、レプリケートされます。
    ///  - なし: 状態はメモリにのみ保持され、レプリケートされません。
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public abstract class Agent : Actor, IAgent
    {
        /// <summary>
        /// Agent の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public Agent(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// このメソッドはアクターがアクティブになると必ず呼び出されます。
        /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // StateManager は、このアクターのプライベート状態ストアです。
            // StateManager に格納されるデータは、揮発性の状態ストレージまたは永続化された状態ストレージを使用するアクターの高可用性を実現するためにレプリケートされます。
            // シリアル化されたオブジェクトは、すべて StateManager に保存できます。
            //詳細については、https://aka.ms/servicefabricactorsstateserialization を参照してください

            return base.OnActivateAsync();
        }

        Task<ChessPieceInfo> IAgent.GetInfoAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Getting current count value as {0}", this.StateManager.GetStateAsync<ChessPieceInfo>("Info"));
            return this.StateManager.GetStateAsync<ChessPieceInfo>("Info");
        }

        Task IAgent.SetInfoAsync(ChessPieceInfo info)
        {
            ActorEventSource.Current.ActorMessage(this, "Setting current count of value to {0}", info);
            return this.StateManager.SetStateAsync<ChessPieceInfo>("Info", info);
        }
        Task IAgent.MoveAsync(int xDirection, int yDirection)
        {
            throw new NotImplementedException();
        }

    }
}
