using CountryActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductActor.Interfaces;

namespace CountryActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IStateActor interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by StateActor objects.
    /// </remarks>
    internal class CountryActor : StatelessActor, ICountryActor
    {
        Task<List<Tuple<string, long>>> ICountryActor.CountCountrySalesAsync()
        {
            string[] products = { "VCR", "Fax", "CassettePlayer", "Camcorder", "GameConsole",
                            "CD", "TV", "Radio", "Phone", "Karaoke"};
            List<Tuple<string, long>> ret = new List<Tuple<string, long>>();
            Parallel.ForEach(products, product =>
            {
                string actorId = this.Id.GetStringId() + "-" + product;
                var proxy = ActorProxy.Create<IProductActor>(new ActorId(actorId), "fabric:/ECommerceApplication");
                ret.Add(new Tuple<string, long>(product, proxy.GetSalesAsync().Result));
            });
            return Task.FromResult(ret);
        }
    }
}
