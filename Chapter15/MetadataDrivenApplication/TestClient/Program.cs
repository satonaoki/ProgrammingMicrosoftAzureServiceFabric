using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shape.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var metadata = new string[] { "Circle", "Rectangle" };
            string applicationName = "fabric:/MetadataDrivenApplication";

            foreach (var action in metadata)
            {
                IShape proxy = ActorProxy.Create<IShape>(ActorId.CreateRandom(), applicationName, action);
                Console.WriteLine(proxy.DrawAsync().Result);
            }

            Console.ReadKey();
        }
    }
}
