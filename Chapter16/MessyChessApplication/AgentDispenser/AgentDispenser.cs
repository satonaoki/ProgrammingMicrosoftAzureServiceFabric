using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using AgentDispenser.Interfaces;
using Microsoft.ServiceFabric.Data;

namespace AgentDispenser
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
    internal class AgentDispenser : Actor, IAgentDispenser
    {
        /// <summary>
        /// AgentDispenser の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public AgentDispenser(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// このメソッドはアクターがアクティブになると必ず呼び出されます。
        /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // StateManager は、このアクターのプライベート状態ストアです。
            // StateManager に格納されるデータは、揮発性の状態ストレージまたは永続化された状態ストレージを使用するアクターの高可用性を実現するためにレプリケートされます。
            // シリアル化されたオブジェクトは、すべて StateManager に保存できます。
            //詳細については、https://aka.ms/servicefabricactorsstateserialization を参照してください

            var result = await this.StateManager.TryGetStateAsync<int>("Count");
            if (!result.HasValue)
            {
                await this.StateManager.SetStateAsync<int>("Count", 0);
            }
        }

        Task<int> IAgentDispenser.GetCountAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Getting current count value as {0}", this.StateManager.GetStateAsync<int>("Count"));
            return this.StateManager.GetStateAsync<int>("Count");
        }

        Task IAgentDispenser.SetCountAsync(int count)
        {
            ActorEventSource.Current.ActorMessage(this, "Setting current count of value to {0}", count);

            return this.StateManager.SetStateAsync<int>("Count", count);
        }
    }
}
