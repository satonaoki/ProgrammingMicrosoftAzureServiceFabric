using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;

namespace SimulatedGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHostName = "***.azure-devices.net";
            string deviceId = "DemoDevice";
            string deviceKey = "***";

            Random rand = new Random();
            var deviceClient = DeviceClient.Create(
                iotHostName,
                new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey)
            );

            while (true)
            {
                double temperature = rand.NextDouble() * 100;
                var temperatureData = new
                {
                    deviceId = deviceId,
                    temperature = temperature
                };
                var message = new Message(Encoding.ASCII.GetBytes(
                    JsonConvert.SerializeObject(temperatureData)));
                deviceClient.SendEventAsync(message).Wait();
                Console.Write(".");
                Thread.Sleep(1000);
            }
        }
    }
}
