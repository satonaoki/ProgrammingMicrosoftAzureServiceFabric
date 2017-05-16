using ActorSwarm.Interfaces;
using Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;

namespace SwarmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("Press [Enter] to start.");
            //Console.ReadLine();

            int size = 50;
            //var swarm = ActorProxy.Create<IActorSwarm>(new ActorId("1"), "fabric:/ActorSwarmApplication", "SpatialSwarm");
            var swarm = ActorProxy.Create<IActorSwarm>(new ActorId("1"), new Uri("fabric:/ActorSwarmApplication/SpatialSwarm"));
            swarm.InitializeAsync(size, 0.65f).Wait();

            int iterations = 2000;
            DateTime refreshTimer = DateTime.Now;

            for (int i = 0; i < iterations; i++)
            {
                swarm.EvolveAsync().Wait();
                if (DateTime.Now - refreshTimer >= TimeSpan.FromSeconds(1))
                {
                    Console.Clear();
                    var state = Shared2DArray<byte>.FromString(swarm.ReadStateStringAsync().Result);
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            switch (state[x, y])
                            {
                                case 0:
                                    Console.Write(" ");
                                    break;
                                case 1:
                                    Console.Write("*");
                                    break;
                                case 2:
                                    Console.Write(".");
                                    break;
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("Iteration: " + i);
                    refreshTimer = DateTime.Now;
                }
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
