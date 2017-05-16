using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace ThrottlingActor.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface IThrottlingActor : IActor
    {
        Task<string> GetAccessTokenAsync(string actionId);
    }
}
