using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace SimulatedGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHostName = "iote2e.azure-devices.net";
            string deviceId = "DemoDevice";
            string deviceKey = "m9SYbvBW0N+aXSbnhBgE5627vq1ix5NvU9Ri0/RqJ5Q=";
            Random rand = new Random();
            var deviceClient = DeviceClient.Create(iotHostName, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));
            while (true)
            {
                double temperature = rand.NextDouble() * 100;
                var temperatureData = new
                {
                    deviceId = deviceId,
                    temperature = temperature
                };
                var message = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(temperatureData)));
                deviceClient.SendEventAsync(message).Wait();
                Console.Write(".");
                Thread.Sleep(1000);
            }
        }
    }
}
