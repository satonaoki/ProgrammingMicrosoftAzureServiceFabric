using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace ActorSwarm.Interfaces
{
    public interface IActorSwarm : IActor
    {
        Task InitializeAsync(int size, float probability);
        Task EvolveAsync();
        Task<string> ReadStateStringAsync();
    }
}
