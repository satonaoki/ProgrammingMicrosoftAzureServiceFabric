using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace FloorActor.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface IFloorActor : IActor
    {
        Task<double> GetTemperatureAsync();
        // Task SetTemperatureAsync(int index, double temperature);
    }
}
