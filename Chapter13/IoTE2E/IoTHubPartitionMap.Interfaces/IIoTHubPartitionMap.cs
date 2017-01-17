using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace IoTHubPartitionMap.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface IIoTHubPartitionMap : IActor
    {
        Task<string> LeaseIoTHubPartitionAsync();
        Task<string> RenewIoTHubPartitionLeaseAsync(string partition);
    }
}
