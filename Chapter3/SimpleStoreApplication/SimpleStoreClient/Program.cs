using System;
using System.ServiceModel;
using Microsoft.ServiceFabric.Services.Client;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Common;
using System.ServiceModel.Channels;
using Microsoft.ServiceFabric.Services.Communication.Wcf;

namespace SimpleStoreClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri serviceUri = new Uri("fabric:/SimpleStoreApplication/ShoppingCartService");
            IServicePartitionResolver serviceResolver = ServicePartitionResolver.GetDefault();
            Binding binding = WcfUtility.CreateTcpClientBinding();
            var wcfClientFactory =
            new WcfCommunicationClientFactory<IShoppingCartService>(
                clientBinding: binding, servicePartitionResolver: serviceResolver);

            for (int i = 1; i <= 3; i++)
            {
                // var shoppingClient = new Client(wcfClientFactory, serviceUri, i);
                var shoppingClient = new Client(wcfClientFactory, serviceUri, "Customer " + i);

                shoppingClient.AddItem(new ShoppingCartItem
                {
                    ProductName = "XBOX ONE",
                    UnitPrice = 329.0,
                    Amount = 2
                }).Wait();

                PrintPartition(shoppingClient);

                var list = shoppingClient.GetItems().Result;
                foreach (var item in list)
                {
                    Console.WriteLine(string.Format("{0}: {1:C2} X {2} = {3:C2}",
                        item.ProductName,
                        item.UnitPrice,
                        item.Amount,
                        item.LineTotal));
                }
            }
            Console.ReadKey();
        }

        private static void PrintPartition(Client client)
        {
            ResolvedServicePartition partition;
            if (client.TryGetLastResolvedServicePartition(out partition))
            {
                Console.WriteLine("Partition ID: " + partition.Info.Id);
            }
        }
    }
}
