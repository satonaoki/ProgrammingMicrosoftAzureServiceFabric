using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Threading;
using ThrottlingActor.Interfaces;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var proxy = ActorProxy.Create<IThrottlingActor>(new ActorId("tenant1"), "fabric:/ThrottlingActorApplication");

            for (int i = 0; i < 3; i++)
                Console.WriteLine("Token: " + proxy.GetAccessTokenAsync("action1").Result);

            Console.ReadKey();
            */

            var proxy = ActorProxy.Create<IThrottlingActor>(new ActorId("tenant1"), "fabric:/ThrottlingActorApplication", "ThrottlingActor");
            var mgtProxy = ActorProxy.Create<IThrottlingActorManagement>(new ActorId("tenant1"), "fabric:/ThrottlingActorApplication", "ThrottlingActor");

            while (true)
            {
                Console.WriteLine("Credits: " + mgtProxy.AddCreditsAsync(150).Result);

                for (int i = 0; i < 3; i++)
                    Console.WriteLine("Token: " + proxy.GetAccessTokenAsync("action1").Result);

                Thread.Sleep(1000);
            }
        }
    }
}
