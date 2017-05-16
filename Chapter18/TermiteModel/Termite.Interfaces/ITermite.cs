using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace Termite.Interfaces
{
    public interface ITermite : IActor
    {
        Task<TermiteState> GetStateAsync();
    }
}
