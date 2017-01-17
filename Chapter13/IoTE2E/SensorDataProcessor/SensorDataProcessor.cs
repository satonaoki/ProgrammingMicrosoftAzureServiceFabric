using IoTHubPartitionMap.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Fabric;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensorDataProcessor
{
    /// <summary>
    /// Service Fabric ランタイムによって、このクラスのインスタンスがサービス レプリカごとに作成されます。
    /// </summary>
    internal sealed class SensorDataProcessor : StatefulService
    {
        public SensorDataProcessor(StatefulServiceContext context)
            : base(context)
        { }

        private class SensorPackage
        {
            public string DeviceId { get; set; }
            public double Temperature { get; set; }
        }

        /// <summary>
        /// このサービス レプリカがクライアント要求やユーザー要求を処理するためのリスナー (HTTP、Service Remoting、WCF など) を作成する、省略可能なオーバーライド。
        /// </summary>
        /// <remarks>
        ///サービスの通信の詳細については、https://aka.ms/servicefabricservicecommunication を参照してください
        /// </remarks>
        /// <returns>リスナーのコレクション。</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// これは、サービス レプリカのメイン エントリ ポイントです。
        /// このメソッドは、サービスのこのレプリカがプライマリになって、書き込み状態になると実行されます。
        /// </summary>
        /// <param name="cancellationToken">Service Fabric がこのサービス レプリカをシャットダウンする必要が生じると、キャンセルされます。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var proxy = ActorProxy.Create<IIoTHubPartitionMap>(new ActorId(1), "fabric:/SensorDataProcessorApplication");
            var eventHubClient = EventHubClient.CreateFromConnectionString("HostName=***.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=***", "messages/events");
            DateTime timeStamp = DateTime.Now;

            while (!cancellationToken.IsCancellationRequested)
            {
                string partition = proxy.LeaseIoTHubPartitionAsync().Result;
                if (partition == "")
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                else
                {
                    var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        EventData eventData = await eventHubReceiver.ReceiveAsync();
                        if (eventData != null)
                        {
                            string data = Encoding.UTF8.GetString(eventData.GetBytes());
                            ServiceEventSource.Current.ServiceMessage(this.Context, "Message: {0}", data);
                            InsertRecord(data);
                        }
                        if (DateTime.Now - timeStamp > TimeSpan.FromSeconds(20))
                        {
                            string lease =
                            proxy.RenewIoTHubPartitionLeaseAsync(partition).Result;
                            if (lease == "")
                                break;
                        }
                    }
                }
            }
        }

        private void InsertRecord(string data)
        {
            var obj = JsonConvert.DeserializeObject<SensorPackage>(data);
            using (var conn = new SqlConnection("Server=tcp:***.database.windows.net,1433;Initial Catalog=***;Persist Security Info=False;User ID=***;Password=***;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT dbo.Sensors (SensorId, Temperature, Timestamp)
                    OUTPUT INSERTED.Id
                    VALUES (@SensorId, @Temperature, @Timestamp)";
                cmd.Parameters.AddWithValue("@SensorId", obj.DeviceId);
                cmd.Parameters.AddWithValue("@Temperature", obj.Temperature);
                cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                conn.Open();
                long insertedId = (long)cmd.ExecuteScalar();
                ServiceEventSource.Current.ServiceMessage(this.Context,"Records ID {0} inserted.", insertedId);
            }
        }
    }
}
