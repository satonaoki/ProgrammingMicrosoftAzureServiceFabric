using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThrottlingActor.Interfaces;
using System;

namespace ThrottlingActor
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
    [ActorService(Name = "ThrottlingActor")]
    internal class ThrottlingActor : Actor, IThrottlingActor, IThrottlingActorManagement
    {
        /// <summary>
        /// ThrottlingActor の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public ThrottlingActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<int> AddCreditsAsync(int credit)
        {
            var remain = await this.StateManager.GetStateAsync<int>("Credit");
            remain += credit;
            await this.StateManager.SetStateAsync<int>("Credit", remain);
            return remain;
        }

        public async Task<string> GetAccessTokenAsync(string actionId)
        {
            string token = "";
            var dict = await this.StateManager.GetStateAsync<Dictionary<string, int>>("ServiceCreditScore");

            if (!dict.ContainsKey(actionId))
            {
                token = "Unsupported action.";
                return token;
            }

            int credit = dict[actionId];
            var remain = await this.StateManager.GetStateAsync<int>("Credit");

            if (remain - credit >= 0)
            {
                remain -= credit;
                await this.StateManager.SetStateAsync<int>("Credit", remain);
                token = "Remaining credit: " + remain;
                // 1. 対称／非対称キーを生成
                // 2. アクセストークンを生成
                // 3. トークンを暗号化し、必要に応じて署名
            }
            else
            {
                token = "Insufficient credit";
            }
            return token;
        }

        /// <summary>
        /// このメソッドはアクターがアクティブになると必ず呼び出されます。
        /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
        /// </summary>
        protected async override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var credit = await this.StateManager.TryGetStateAsync<List<string>>("Credit");

            if (!credit.HasValue)
            {
                await this.StateManager.SetStateAsync<int>("Credit", 100);
                await this.StateManager.SetStateAsync<Dictionary<string, int>>(
                   "ServiceCreditScore",
                    new Dictionary<string, int> {
                        { "action1", 50 },
                        { "action2", 25 },
                        { "action3", 10 }
                    }
                );
            }
        }
    }
}
