using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace AgentDispenser.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface IAgentDispenser : IActor
    {
        Task<int> GetCountAsync();

        Task SetCountAsync(int count);
    }
}
