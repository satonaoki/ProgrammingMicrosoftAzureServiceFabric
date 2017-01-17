using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace SensorActor.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface ISensorActor : IActor
    {
        Task<double> GetTemperatureAsync();
        Task SetTemperatureAsync(double temperature);
        Task<int> GetIndexAsync();
        Task SetIndexAsync(int index);
    }
}
