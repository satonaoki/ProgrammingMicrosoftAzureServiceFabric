using System;
using CalculatorService;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CalculatorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ICalculatorService calculatorClient =
                ServiceProxy.Create<ICalculatorService>
                (new Uri("fabric:/CalculatorApplication1/CalculatorService"));
            var result = calculatorClient.Add(1, 2).Result.ToString();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
