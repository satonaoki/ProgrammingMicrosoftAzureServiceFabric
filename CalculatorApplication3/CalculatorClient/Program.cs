using CalculatorService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.ServiceModel.Channels;

namespace CalculatorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri serviceUri = new Uri("fabric:/CalculatorApplication3/CalculatorService");
            IServicePartitionResolver serviceResolver = ServicePartitionResolver.GetDefault();
            Binding binding = WcfUtility.CreateTcpClientBinding();
            var wcfClientFactory = new WcfCommunicationClientFactory<ICalculatorService>(
                clientBinding: binding, servicePartitionResolver: serviceResolver);
            var calcClient = new Client(wcfClientFactory, serviceUri);
            Console.WriteLine(calcClient.Add(3, 5).Result);
            Console.ReadKey();
        }

    }
}