using FloorActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using SensorActor.Interfaces;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SensorActor
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
    internal class SensorActor : Actor, ISensorActor
    {
        /// <summary>
        /// SensorActor の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public SensorActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<int> GetIndexAsync()
        {
            return this.StateManager.GetStateAsync<int>("Index");
        }

        public Task<double> GetTemperatureAsync()
        {
            ActorEventSource.Current.ActorMessage(this,
                "Getting current temperature value as {0}",
            this.StateManager.GetStateAsync<double>("Temperature"));
            return this.StateManager.GetStateAsync<double>("Temperature");
        }

        public async Task SetIndexAsync(int index)
        {
            await this.StateManager.SetStateAsync<int>("Index", index);
        }

        public async Task SetTemperatureAsync(double temperature)
        {
            ActorEventSource.Current.ActorMessage(this,
                "Setting current temperature of value to {0}", temperature);
            await this.StateManager.SetStateAsync<double>("Temperature", temperature);
            
            /*
            var proxy = ActorProxy.Create<IFloorActor>(new ActorId(2016), "fabric:/SensorAggregationApplication");
            var index = await this.StateManager.GetStateAsync<int>("Index");
            await proxy.SetTemperatureAsync(index, temperature);
            */
        }

        /// <summary>
        /// このメソッドはアクターがアクティブになると必ず呼び出されます。
        /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
        /// </summary>
        protected async override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // StateManager は、このアクターのプライベート状態ストアです。
            // StateManager に格納されるデータは、揮発性の状態ストレージまたは永続化された状態ストレージを使用するアクターの高可用性を実現するためにレプリケートされます。
            // シリアル化されたオブジェクトは、すべて StateManager に保存できます。
            //詳細については、https://aka.ms/servicefabricactorsstateserialization を参照してください

            var temperature = await this.StateManager.TryGetStateAsync<double>("Temperature");

            if (!temperature.HasValue)
            {
                await this.StateManager.SetStateAsync<double>("Temperature", 0);
            }
        }
    }
}
