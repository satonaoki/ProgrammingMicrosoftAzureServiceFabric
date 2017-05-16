using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MspService.Interfaces
{
    public interface MapService
    {
        Task<List<POI>> GetPOIs(float lat, float lon);
    }
}
