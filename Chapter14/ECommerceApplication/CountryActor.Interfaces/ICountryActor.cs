using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryActor.Interfaces
{
    /// <summary>
    /// このインターフェイスは、アクターが公開するメソッドを定義します。
    /// クライアントはこのインターフェイスを使用して、インターフェイスを実装するアクターと対話します。
    /// </summary>
    public interface ICountryActor : IActor
    {
        Task<List<Tuple<string, long>>> CountCountrySalesAsync();
    }
}
