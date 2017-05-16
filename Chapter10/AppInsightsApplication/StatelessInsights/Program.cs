using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Diagnostics;


namespace StatelessInsights
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
                ServiceRuntime.RegisterServiceAsync("StatelessInsightsType",
                    context => new StatelessInsights(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(StatelessInsights).Name);

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
