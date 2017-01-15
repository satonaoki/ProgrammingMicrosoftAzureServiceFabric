using IoTHubParitionMap.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensorDataProcessor
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class SensorDataProcessor : StatefulService
    {
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        protected override async Task RunAsync(CancellationToken cancelServicePartitionReplica)
        {
            var proxy = ActorProxy.Create<IIoTHubPartitionMap>(new ActorId(1), "fabric:/SensorDataProcessorApplication");
            var eventHubClient = EventHubClient.CreateFromConnectionString("HostName=iote2e.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=++Fev//TfVOkRUYa4s2bIJA4gad2UvPHQ2K+razIYP8=", "messages/events");
            DateTime timeStamp = DateTime.Now;

            while (!cancelServicePartitionReplica.IsCancellationRequested)
            {
                string partition = proxy.LeaseTHubPartitionAsync().Result;
                if (partition == "")
                    await Task.Delay(TimeSpan.FromSeconds(15), cancelServicePartitionReplica);
                else
                {
                    var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
                    while (!cancelServicePartitionReplica.IsCancellationRequested)
                    {

                        EventData eventData = await eventHubReceiver.ReceiveAsync();
                        if (eventData != null)
                        {
                            string data = Encoding.UTF8.GetString(eventData.GetBytes());
                            ServiceEventSource.Current.ServiceMessage(this, "Message: {0}", data);
                            InsertRecord(data);
                        }
                        if (DateTime.Now - timeStamp > TimeSpan.FromSeconds(20))
                        {
                            string lease = proxy.RenewIoTHubPartitionLeaseAsync(partition).Result;
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

    using (var conn = new SqlConnection("[SQL Connection string]"))
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

        ServiceEventSource.Current.ServiceMessage(this, "Records ID {0} inserted.", insertedId);
    }
}
private class SensorPackage
{
    public string DeviceId { get; set; }
    public double Temperature { get; set; }
}
    }
}
