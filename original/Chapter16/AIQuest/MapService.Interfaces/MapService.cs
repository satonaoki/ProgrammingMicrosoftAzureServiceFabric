using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MspService.Interfaces
{
    public interface MapService
    {
        Task<List<POI>> GetPOIs(float lat, float lon);
    }
}
