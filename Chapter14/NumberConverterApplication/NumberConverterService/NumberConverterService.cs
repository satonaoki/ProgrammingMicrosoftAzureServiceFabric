using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceBus.Messaging;

namespace NumberConverterService
{
    /// <summary>
    /// Service Fabric ランタイムによって、このクラスのインスタンスがサービス インスタンスごとに作成されます。
    /// </summary>
    internal sealed class NumberConverterService : StatelessService
    {
        public NumberConverterService(StatelessServiceContext context)
            : base(context)
        { }

        EventHubClient eventHubClient;

        /// <summary>
        /// このサービス レプリカがクライアントやユーザーの要求を処理するために、リスナー (TCP、HTTP など) を作成するようオーバーライドします (省略可能)。
        /// </summary>
        /// <returns>リスナーのコレクション。</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// これは、サービス インスタンスのメイン エントリ ポイントです。
        /// </summary>
        /// <param name="cancellationToken">Service Fabric がこのサービス インスタンスをシャットダウンする必要が生じると、キャンセルされます。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            string eventHubName = "***";
            string eventHubConnectionString = "Endpoint=sb://***.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=***";

            string storageAccountName = "***";
            string storageAccountKey = "***";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}",
                ++iterations);
                if (iterations > 1000) iterations = 0;
                eventHubClient = EventHubClient.CreateFromConnectionString(
                eventHubConnectionString, eventHubName);
                string eventProcessorHostName = Guid.NewGuid().ToString();
                EventProcessorHost eventProcessorHost =
                new EventProcessorHost(eventProcessorHostName, eventHubName,
                EventHubConsumerGroup.DefaultGroupName,
                eventHubConnectionString,
                storageConnectionString);
                await
                eventProcessorHost.RegisterEventProcessorAsync<NumberConverter>();
            }
        }
    }
}
