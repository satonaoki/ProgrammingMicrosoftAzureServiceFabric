using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ProductActor.Interfaces;
using System;
using System.Threading.Tasks;

namespace ProductActor
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
    internal class ProductActor : Actor, IProductActor
    {
        /// <summary>
        /// ProductActor の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public ProductActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<int> GetSalesAsync()
        {
            var data = await this.StateManager.GetStateAsync<int>("Sales");
            ActorEventSource.Current.ActorMessage(this, "Getting current sales value as {0}", data);
            return data;
        }

        public async Task SellAsync()
        {
            var data = await this.StateManager.GetStateAsync<int>("Sales");
            data += 1;
            await this.StateManager.SetStateAsync<int>("Sales", data);
        }

        /// <summary>
        /// このメソッドはアクターがアクティブになると必ず呼び出されます。
        /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
        /// </summary>
        protected async override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var sales = await this.StateManager.TryGetStateAsync<int>("Sales");

            if (!sales.HasValue)
            {
                await this.StateManager.SetStateAsync<int>("Sales", 0);
            }
        }
    }
}
