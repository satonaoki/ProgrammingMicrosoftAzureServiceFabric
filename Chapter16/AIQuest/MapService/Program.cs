using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace MapService
{
    internal static class Program
    {
        /// <summary>
        /// これは、サービス ホスト プロセスのエントリ ポイントです。
        /// </summary>
        private static void Main()
        {
            try
            {
                // ServiceManifest.XML ファイルは、1 つ以上のサービス型の名前を定義します。
                // サービスを登録すると、サービス型の名前が .NET 型にマップされます。
                // Service Fabric がこのサービス型のインスタンスを作成すると、
                // このホスト プロセスでクラスのインスタンスが作成されます。

                ServiceRuntime.RegisterServiceAsync("MapServiceType",
                    context => new MapService(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(MapService).Name);

                // サービスが実行を続けるために、このホスト プロセスが終了しないようにします。
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
