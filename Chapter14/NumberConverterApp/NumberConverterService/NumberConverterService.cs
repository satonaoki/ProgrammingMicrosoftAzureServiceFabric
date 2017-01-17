using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace NumberConverterService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class NumberConverterService : StatelessService
    {
        EventHubClient eventHubClient;

        public NumberConverterService(StatelessServiceContext serviceContext) : base(serviceContext)
        {
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            // TODO: If your service needs to handle user requests, return a list of ServiceReplicaListeners here.
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancelServiceInstance">Canceled when Service Fabric terminates this instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            string eventHubName = "naoki1";
            string eventHubConnectionString = "Endpoint=sb://naokieb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=s55nJTdDLl1wEx2vcomzeTuGyOxbvASSp5Vkkk/eM3I=";

            string storageAccountName = "naokidemo";
            string storageAccountKey = "vtMA2vgtI6K8U5kFn12h8yl1Lxa2DQbb1Ajy2VVNaxNoVaPw9IZm47S5i0IMcU7SbSW6GNTHvenwr5XNTNrj+w==";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                if (iterations > 1000) iterations = 0;

                eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);
                string eventProcessorHostName = Guid.NewGuid().ToString();
                EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
                await eventProcessorHost.RegisterEventProcessorAsync<NumberConverter>();
            }
        }
    }
}
