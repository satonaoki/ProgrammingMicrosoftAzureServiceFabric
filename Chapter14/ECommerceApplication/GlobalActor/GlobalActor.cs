using GlobalActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CountryActor.Interfaces;
using System.Collections.Concurrent;

namespace GlobalActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The INationActor interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by NationActor objects.
    /// </remarks>
    internal class GlobalActor : StatelessActor, IGlobalActor
    {
        Task<List<Tuple<string, long>>> IGlobalActor.CountGlobalSalesAsync()
        {
            string[] countries = { "US", "China", "Australia" };
            ConcurrentDictionary<string, long> sales = new ConcurrentDictionary<string, long>();
            Parallel.ForEach(countries, country =>
            {
                var proxy = ActorProxy.Create<ICountryActor>(new ActorId(country), "fabric:/ECommerceApplication");
                var countrySales = proxy.CountCountrySalesAsync().Result;
                foreach (var tuple in countrySales)
                {
                    sales.AddOrUpdate(tuple.Item1, tuple.Item2, (key, oldValue) => oldValue + tuple.Item2);
                }
            });
            var list = from entry in sales
                       orderby entry.Value descending
                       select new Tuple<string, long>(entry.Key, entry.Value);
            return Task.FromResult(list.ToList());
        }
    }
}
