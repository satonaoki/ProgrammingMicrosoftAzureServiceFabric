using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlobalActor.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface IGlobalActor : IActor
    {
        Task<List<Tuple<string, long>>> CountGlobalSalesAsync();
    }
}
