using Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStoreClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri ServiceName = new Uri("fabric:/SimpleStoreApplication/ShoppingCartService");

            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() => new FabricClient());
            NetTcpBinding binding = CreateClientConnectionBinding();

            for (int i = 1; i <= 3; i++)
            {
                Client shoppingClient = new Client(new WcfCommunicationClientFactory<IShoppingCartService>
                (serviceResolver, binding, null),
                            ServiceName, "Customer " + i);
                shoppingClient.AddItem(new ShoppingCartItem
                {
                    ProductName = "XBOX ONE (" + i.ToString() + ")",
                    UnitPrice = 329.0,
                    Amount = 2
                }).Wait();
                shoppingClient.AddItem(new ShoppingCartItem
                {
                    ProductName = "Halo 5 (" + i.ToString() + ")",
                    UnitPrice = 59.99,
                    Amount = 1
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
        }
        private static NetTcpBinding CreateClientConnectionBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;

            return binding;
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
