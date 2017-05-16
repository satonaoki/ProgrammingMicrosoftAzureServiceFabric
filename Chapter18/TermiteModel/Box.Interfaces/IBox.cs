using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Box.Interfaces
{
    public interface IBox : IService
    {
        Task<bool> TryPickUpWoodChipAsync(int x, int y);
        Task<bool> TryPutDonwWoodChipAsync(int x, int y);
        Task<List<int>> ReadBox();
        Task ResetBox();
    }
}
