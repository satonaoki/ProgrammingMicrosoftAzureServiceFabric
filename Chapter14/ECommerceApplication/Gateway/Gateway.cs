using Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ProductActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class Gateway : StatelessService, IWebSocketConnectionHandler
    {
        public Gateway(StatelessServiceContext serviceContext) : base(serviceContext)
        {
        }

        public async Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, CancellationToken cancellationToken)
        {
            ProtobufWsSerializer mserializer = new ProtobufWsSerializer();

            WsRequestMessage mrequest = await mserializer.DeserializeAsync<WsRequestMessage>(wsrequest);

            switch (mrequest.Operation)
            {
                case "sell":
                    {
                        IWsSerializer pserializer = SerializerFactory.CreateSerializer();
                        PostSalesModel payload = await pserializer.DeserializeAsync<PostSalesModel>(mrequest.Value);
                        //await this.PurchaseProduct(payload.ProductId, payload.Quantity);
                        var id = payload.Country + "-" + payload.Product;
                        var candidate = ActorProxy.Create<IProductActor>(new ActorId(id), "fabric:/ECommerceApplication");
                        await candidate.SellAsync();
                    }
                    break;
            }

            WsResponseMessage mresponse = new WsResponseMessage
            {
                Result = WsResult.Success
            };

            return await mserializer.SerializeAsync(mresponse);
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] {
                new ServiceInstanceListener(
                    context => new WebSocketListener("SalesServiceWS", context, () => this),
                    "Websocket")
            };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancelServiceInstance">Canceled when Service Fabric terminates this instance.</param>
        protected override async Task RunAsync(CancellationToken cancelServiceInstance)
        {
            // TODO: Replace the following sample code with your own logic.

            int iterations = 0;
            // This service instance continues processing until the instance is terminated.
            while (!cancelServiceInstance.IsCancellationRequested)
            {

                // Log what the service is doing
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", iterations++);

                // Pause for 1 second before continue processing.
                await Task.Delay(TimeSpan.FromSeconds(1), cancelServiceInstance);
            }
        }
    }
}
