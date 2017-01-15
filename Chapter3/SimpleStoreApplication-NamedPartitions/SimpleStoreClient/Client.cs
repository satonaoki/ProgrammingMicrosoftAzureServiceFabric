using Common;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStoreClient
{
    public class Client : ServicePartitionClient<WcfCommunicationClient<IShoppingCartService>>, IShoppingCartService
    {
        public Client(WcfCommunicationClientFactory<IShoppingCartService> clientFactory,
                               Uri serviceName, string customerId)
         : base(clientFactory, serviceName, customerId)
        {
        }

        public Task AddItem(ShoppingCartItem item)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.AddItem(item));
        }

        public Task DeleteItem(string productName)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.DeleteItem(productName));
        }

        public Task<List<ShoppingCartItem>> GetItems()
        {
            return this.InvokeWithRetryAsync(client => client.Channel.GetItems());
        }
    }
}
