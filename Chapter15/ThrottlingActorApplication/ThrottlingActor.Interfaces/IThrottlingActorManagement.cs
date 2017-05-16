using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace ThrottlingActor.Interfaces
{
    public interface IThrottlingActorManagement : IActor
    {
        Task<int> AddCreditsAsync(int credit);
    }
}
