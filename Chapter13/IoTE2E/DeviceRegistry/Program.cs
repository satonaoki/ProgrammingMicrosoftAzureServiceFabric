using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=iote2e.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=++Fev//TfVOkRUYa4s2bIJA4gad2UvPHQ2K+razIYP8=");
            string deviceId = "DemoDevice";
            var device = registryManager.AddDeviceAsync(new Device(deviceId)).Result;
            Console.WriteLine("Device Key: " + device.Authentication.SymmetricKey.PrimaryKey);
            Console.ReadLine();
        }
    }
}
