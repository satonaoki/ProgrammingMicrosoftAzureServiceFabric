using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace ProductActor.Interfaces
{
    /// <summary>
    /// This interface represents the actions a client app can perform on an actor.
    /// It MUST derive from IActor and all methods MUST return a Task.
    /// </summary>
    public interface IProductActor : IActor
    {
        Task<int> GetSalesAsync();

        Task SellAsync();
    }
}
