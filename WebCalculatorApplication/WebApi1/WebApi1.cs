using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace WebApi1
{
    /// <summary>
    /// FabricRuntime は、サービス型インスタンスごとにこのクラスのインスタンスを作成します。
    /// </summary>
    internal sealed class WebApi1 : StatelessService
    {
        public WebApi1(StatelessServiceContext context)
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
                new ServiceInstanceListener(serviceContext => new OwinCommunicationListener(Startup.ConfigureApp, serviceContext, ServiceEventSource.Current, "ServiceEndpoint"))
            };
        }
    }
}
