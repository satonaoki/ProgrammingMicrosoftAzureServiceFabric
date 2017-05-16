using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ApplicationInsights;

namespace StatelessInsights
{
    /// <summary>
    /// Service Fabric ランタイムによって、このクラスのインスタンスがサービス インスタンスごとに作成されます。
    /// </summary>
    internal sealed class StatelessInsights : StatelessService
    {
        public StatelessInsights(StatelessServiceContext context)
            : base(context)
        {

        }

        /// <summary>
        /// このサービス レプリカがクライアントやユーザーの要求を処理するために、リスナー (TCP、HTTP など) を作成するようオーバーライドします (省略可能)。
        /// </summary>
        /// <returns>リスナーのコレクション。</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// これは、サービス インスタンスのメイン エントリ ポイントです。
        /// </summary>
        /// <param name="cancellationToken">Service Fabric がこのサービス インスタンスをシャットダウンする必要が生じると、キャンセルされます。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var client = new TelemetryClient();
            client.InstrumentationKey = "";

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                client.TrackMetric("MyTest", iterations, new Dictionary<string, string>
                {
                    { "ServiceName", this.Context.ServiceName.AbsoluteUri }
                });

                if (iterations > 1000) iterations = 0;

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
