using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThrottlingActor.Interfaces;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = ActorProxy.Create<IThrottlingActor>(new ActorId("tenant1"), "fabric:/ThrottlingActorApplication", "ThrottlingActor");
            var mgtProxy = ActorProxy.Create<IThrottlingActorManagement>(new ActorId("tenant1"), "fabric:/ThrottlingActorApplication", "ThrottlingActor");

            while (true)
            {
                Console.WriteLine("Credits: " + mgtProxy.AddCreditsAsync(150).Result);

                for (int i = 0; i < 3; i++)
                    Console.WriteLine("Token:" + proxy.GetAccessTokenAsync("action1").Result);
                Thread.Sleep(1000);
            }
        }
    }
}
