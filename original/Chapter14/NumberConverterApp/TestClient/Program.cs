using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static string eventHubName1 = "hub-1";
        static string connectionString1 = "[Event Hub Connection string 1]";
        static string eventHubName2 = "hub-2";
        static string connectionString2 = "[Event Hub Connection string 2]";
        static Random random = new Random();
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem((a) => SendMessages());
            ThreadPool.QueueUserWorkItem((a) => ReceiveMessages("0"));
            ThreadPool.QueueUserWorkItem((a) => ReceiveMessages("1"));
            ThreadPool.QueueUserWorkItem((a) => ReceiveMessages("2"));
            ThreadPool.QueueUserWorkItem((a) => ReceiveMessages("3"));
            Console.ReadKey();
        }
        static void SendMessages()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString1, eventHubName1);
            while (true)
            {
                try
                {
                    var message = random.Next(0, int.MaxValue);
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                    eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(message.ToString())));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(200);
            }
        }
        static void ReceiveMessages(string partitionId)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString2, eventHubName2);
            var defaultConsumberGroup = eventHubClient.GetDefaultConsumerGroup();
            var receiver = defaultConsumberGroup.CreateReceiver(partitionId);
            while (true)
            {
                var data = receiver.Receive();
                if (data != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0} - {2} > Data: {1}", DateTime.Now, Encoding.UTF8.GetString(data.GetBytes()), partitionId);
                    Console.ResetColor();
                }
            }
        }
    }
}
