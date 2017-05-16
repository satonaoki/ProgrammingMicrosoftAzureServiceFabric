using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AIQuestWeb
{
    /// <summary>
    /// FabricRuntime は、サービス型インスタンスごとにこのクラスのインスタンスを作成します。
    /// </summary>
    internal sealed class AIQuestWeb : StatelessService
    {
        public AIQuestWeb(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// このサービス インスタンスのリスナー (tcp、http など) を作成するためにオーバーライドします (省略可能)。
        /// </summary>
        /// <returns>リスナーのコレクション。</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new WebListenerCommunicationListener(serviceContext, "ServiceEndpoint", url =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting WebListener on {url}");

                        return new WebHostBuilder().UseWebListener()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}