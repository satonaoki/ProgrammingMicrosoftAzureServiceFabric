using Circle.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Rectangle.Interfaces;
using Shape.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var metadata = new string[] { "Circle", "Rectangle" };
            string applicationName = "fabric:/MetadataDrvienApplication";
            foreach (var action in metadata)
            {
                IShape proxy = ActorProxy.Create<IShape>(ActorId.NewId(), applicationName, action);
                Console.WriteLine(proxy.DrawAsync().Result);
            }
        }
    }
}
