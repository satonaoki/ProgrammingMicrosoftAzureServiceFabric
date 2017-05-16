using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Shape.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface IShape : IActor
    {
        Task<string> DrawAsync();
    }
}
