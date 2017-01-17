using IoTHubPartitionMap.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubPartitionMap
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
    internal class IoTHubPartitionMap : Actor, IIoTHubPartitionMap
    {
        IActorTimer mTimer;

        /// <summary>
        /// IoTHubPartitionMap の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actorService">このアクター インスタンスをホストする Microsoft.ServiceFabric.Actors.Runtime.ActorService。</param>
        /// <param name="actorId">このアクター インスタンスの Microsoft.ServiceFabric.Actors.ActorId。</param>
        public IoTHubPartitionMap(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<string> LeaseIoTHubPartitionAsync()
        {
            string ret = "";
            var partitions = await this.StateManager.GetStateAsync<List<string>>("PartitionNames");
            var dict = await this.StateManager.GetStateAsync<Dictionary<string, DateTime>>("PartitionLeases");

            foreach (string partition in partitions)
            {
                if (!dict.ContainsKey(partition))
                {
                    dict.Add(partition, DateTime.Now);
                    await this.StateManager.SetStateAsync<Dictionary<string, DateTime>>("PartitionLeases", dict);
                    ret = partition;
                    break;
                }
            }
            return ret;
        }

        public async Task<string> RenewIoTHubPartitionLeaseAsync(string partition)
        {
            string ret = "";
            var dict = await
            this.StateManager.GetStateAsync<Dictionary<string, DateTime>>(
            "PartitionLeases");
            if (dict.ContainsKey(partition))
            {
                dict[partition] = DateTime.Now;
                await this.StateManager.SetStateAsync<Dictionary<string, DateTime>>(
                "PartitionLeases", dict);
                ret = partition;
            }
            return ret;
        }

        /// <summary>
        /// このメソッドはアクターがアクティブになると必ず呼び出されます。
        /// アクターは、メソッドのいずれかが初めて呼び出されるときにアクティブ化されます。
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var partitionNames = await this.StateManager.TryGetStateAsync<List<string>>("PartitionNames");

            if (!partitionNames.HasValue)
            {
                await this.StateManager.SetStateAsync<List<string>>("PartitionNames", new List<string>());
                await this.StateManager.SetStateAsync<Dictionary<string, DateTime>>("PartitionLeases", new Dictionary<string, DateTime>());
                ResetPartitionNames();
                mTimer = RegisterTimer(CheckLease, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            }
        }

        private async Task CheckLease(Object state)
        {
            var dict = await this.StateManager.GetStateAsync<Dictionary<string, DateTime>>("PartitionLeases");
            List<string> keys = dict.Keys.ToList();
            foreach (string key in keys)
            {
                if (DateTime.Now - dict[key] >= TimeSpan.FromSeconds(60))
                    dict.Remove(key);
            }
            await this.StateManager.SetStateAsync<Dictionary<string, DateTime>>("PartitionLeases", dict);
        }

        protected override Task OnDeactivateAsync()
        {
            if (mTimer != null)
                UnregisterTimer(mTimer);
            return base.OnDeactivateAsync();
        }

        private async void ResetPartitionNames()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(
            "HostName=***.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=***", "messages/events");
            var partitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            var saved_partitions = await this.StateManager.GetStateAsync<List<string>>("PartitionNames");
            foreach (string partition in partitions)
            {
                saved_partitions.Add(partition);
            }
            await this.StateManager.SetStateAsync <List<string>>("PartitionNames", saved_partitions);
        }
    }
}
