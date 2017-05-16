using Common;
using Gateway;
using GlobalActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] products = { "VCR", "Fax", "CassettePlayer", "Camcorder", "GameConsole",
                            "CD", "TV", "Radio", "Phone", "Karaoke"};
            string[] countries = { "US", "China", "Australia" };
            Random rand = new Random();

            Parallel.For(0, 10, i =>
            {
                SimulateSales(countries[rand.Next(0, countries.Length)], products[rand.Next(0, products.Length)]);
            });
            
            var nation = ActorProxy.Create<IGlobalActor>(new ActorId("1"), "fabric:/ECommerceApplication");
            var list = nation.CountGlobalSalesAsync().Result;
            foreach (var result in list)
            {
                Console.WriteLine(result.Item1.PadLeft(18,' ') + ": " + result.Item2);
            }
        }
        static void SimulateSales(string country, string product)
        {
            using (GatewayWebSocketClient websocketClient = new GatewayWebSocketClient())
            {
                websocketClient.ConnectAsync("ws://localhost:31009/SalesServiceWS").Wait();
                PostSalesModel postSalesModel = new PostSalesModel
                {
                    Product = product,
                    Country = country
                };

                IWsSerializer serializer = SerializerFactory.CreateSerializer();
                byte[] payload = serializer.SerializeAsync(postSalesModel).Result;

                WsRequestMessage mreq = new WsRequestMessage
                {
                    Operation = "sell",
                    Value = payload
                }; 

                WsResponseMessage mresp = websocketClient.SendReceiveAsync(mreq, CancellationToken.None).Result;
                if (mresp.Result == WsResult.Error)
                    Console.WriteLine("Error: {0}", Encoding.UTF8.GetString(mresp.Value));
            }
         }
    }
}
