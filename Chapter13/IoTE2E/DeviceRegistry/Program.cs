using Microsoft.Azure.Devices;
using System;

namespace DeviceRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=***.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=***");
            string deviceId = "DemoDevice";
            var device =
            registryManager.AddDeviceAsync(new Device(deviceId)).Result;
            Console.WriteLine("Device Key: " +
            device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
