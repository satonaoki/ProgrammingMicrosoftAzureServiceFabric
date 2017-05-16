using CountryActor.Interfaces;
using GlobalActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalActor
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
    internal class GlobalActor : Actor, IGlobalActor
    {
        /// <summary>
        /// GlobalActor の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public GlobalActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<List<Tuple<string, long>>> CountGlobalSalesAsync()
        {
            string[] countries = { "US", "China", "Australia" };
            ConcurrentDictionary<string, long> sales = new ConcurrentDictionary<string, long>();

            Parallel.ForEach(countries, country =>
            {
                var proxy = ActorProxy.Create<ICountryActor>(
                new ActorId(country), "fabric:/ECommerceApplication");
                var countrySales = proxy.CountCountrySalesAsync().Result;
                foreach (var tuple in countrySales)
                {
                    sales.AddOrUpdate(tuple.Item1, tuple.Item2,
                    (key, oldValue) => oldValue + tuple.Item2);
                }
            });

            var list = from entry in sales
                       orderby entry.Value descending
                       select new Tuple<string, long>(entry.Key, entry.Value);

            return Task.FromResult(list.ToList());
        }
    }
}
