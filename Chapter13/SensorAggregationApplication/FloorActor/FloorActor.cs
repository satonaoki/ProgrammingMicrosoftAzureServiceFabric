using FloorActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using SensorActor.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace FloorActor
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
    internal class FloorActor : Actor, IFloorActor
    {
        /// <summary>
        /// FloorActor の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public FloorActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<double> GetTemperatureAsync()
        {
            /*
            var temp =
            await this.StateManager.GetStateAsync<double[]>("Temperature");
            return temp.Average();
            */

            Task<double>[] tasks = new Task<double>[1000];
            double[] readings = new double[1000];
            Parallel.For(0, 1000, i =>
            {
                var proxy = ActorProxy.Create<ISensorActor>(new ActorId(i), "fabric:/SensorAggregationApplication");
                tasks[i] = proxy.GetTemperatureAsync();
            });
            Task.WaitAll(tasks);

            Parallel.For(0, 1000, i =>
            {
                readings[i] = tasks[i].Result;
            });
            return await Task.FromResult(readings.Average());
        }

    /*
     * public async Task SetTemperatureAsync(int index, double temperature)
    {
        var temp =
        await this.StateManager.GetStateAsync<double[]>("Temperature");
        temp[index] = temperature;
        await this.StateManager.SetStateAsync<double[]>("Temperature", temp);
    }
    */

    /// <summary>
    /// このメソッドはアクターがアクティブになると必ず呼び出されます。
    /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
    /// </summary>
    protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            /*
             * var temperature = await this.StateManager.TryGetStateAsync<double[]>("Temperature");

            if (!temperature.HasValue)
            {
                await this.StateManager.SetStateAsync<double[]>("Temperature", new double[1000]);
            }
            */

            var temperature = await this.StateManager.TryGetStateAsync<double>("Temperature");

            if (!temperature.HasValue)
            {
                await this.StateManager.SetStateAsync<double>("Temperature", 0);
            }
        }
    }
}
