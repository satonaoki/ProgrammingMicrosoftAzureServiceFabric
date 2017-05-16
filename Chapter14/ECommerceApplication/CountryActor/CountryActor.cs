using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using CountryActor.Interfaces;
using ProductActor.Interfaces;

namespace CountryActor
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
    internal class CountryActor : Actor, ICountryActor
    {
        /// <summary>
        /// CountryActor の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public CountryActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<List<Tuple<string, long>>> CountCountrySalesAsync()
        {
            string[] products = {
                "VCR", "Fax", "CassettePlayer", "Camcorder", "GameConsole",
                "CD", "TV", "Radio", "Phone", "Karaoke"};

            List<Tuple<string, long>> ret = new List<Tuple<string, long>>();

            Parallel.ForEach(products, product =>
            {
                string actorId = this.Id.GetStringId() + "-" + product;
                var proxy = ActorProxy.Create<IProductActor>(
                new ActorId(actorId), "fabric:/ECommerceApplication");
                ret.Add(new Tuple<string, long>(product,
                proxy.GetSalesAsync().Result));
            });

            return Task.FromResult(ret);
        }
    }
}
