using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace CalculatorService
{
    public interface ICalculatorService : IService
    {
        Task<string> Add(int a, int b);
        Task<string> Subtract(int a, int b);
    }
}