using Box.Interfaces;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;

namespace Box
{
    internal sealed class Box : StatefulService, IBox
    {
        private static Random mRand = new Random();
        private const string mDictionaryName = "box";
        private const int size = 100;

        public Box(StatefulServiceContext context) : base(context)
        {      
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //return new ServiceReplicaListener[0];
            return new[]
            {
                new ServiceReplicaListener(context => this.CreateServiceRemotingListener<Box>(context))  
            };
        }

        public async Task<List<int>> ReadBox()
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>(mDictionaryName);
            List<int> ret = new List<int>();       

            using (var tx = this.StateManager.CreateTransaction())
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        var value = await myDictionary.TryGetValueAsync(tx, x + "-" + y);

                        if (value.HasValue)
                            ret.Add(value.Value);
                        else
                            ret.Add(0);
                    }
                }
                await tx.CommitAsync();
            }
            return ret;
        }

        public async Task ResetBox()
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>(mDictionaryName);
            await myDictionary.ClearAsync();

            using (var tx = this.StateManager.CreateTransaction())
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        await myDictionary.SetAsync(tx, x + "-" + y, 0);
                    }
                }
                for (int i = 0; i < size * size / 6; i++)
                {
                    var x = mRand.Next(0, size);
                    var y = mRand.Next(0, size);
                    await myDictionary.SetAsync(tx, x + "-" + y, 1);
                }
                await tx.CommitAsync();
            }
        }

        public async Task<bool> TryPickUpWoodChipAsync(int x, int y)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>(mDictionaryName);
            var ret = false;
            using (var tx = this.StateManager.CreateTransaction())
            {
                string key = x + "-" + y;
                var result = await myDictionary.TryGetValueAsync(tx, key);
                if (result.HasValue && result.Value == 1)
                {
                    ret = await myDictionary.TryUpdateAsync(tx, key, 0, 1);
                }
                await tx.CommitAsync();
            }
            return ret;
        }

        public async Task<bool> TryPutDonwWoodChipAsync(int x, int y)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>(mDictionaryName);
            var ret = false;
            int lastX = x;
            int lastY = y;
            using (var tx = this.StateManager.CreateTransaction())
            {
                string key = x + "-" + y;
                var result = await myDictionary.TryGetValueAsync(tx, key);
                if (result.HasValue && result.Value == 1)
                {
                    for (int r = 1; r < 2; r++)
                    {
                        double angle = mRand.NextDouble() * Math.PI * 2;
                        for (double a = angle; a < Math.PI * 2 + angle; a += 0.01)
                        {
                            int newX = (int)(x + r * Math.Cos(a));
                            int newY = (int)(y + r * Math.Sin(a));
                            if ((newX != lastX || newY != lastY)
                                && newX >= 0 && newY >= 0 && newX < size && newY < size)
                            {
                                lastX = newX;
                                lastY = newY;
                                string testKey = newX + "-" + newY;
                                var neighbour = myDictionary.TryGetValueAsync(tx, testKey).Result;
                                if (neighbour.HasValue && neighbour.Value == 0)
                                {
                                    ret = await myDictionary.TryUpdateAsync(tx, testKey, 1, 0);
                                    if (ret)
                                        break;
                                }
                            }
                        }
                        if (ret)
                            break;
                    }
                }
                await tx.CommitAsync();
            }
            return ret;
        }
    }
}
