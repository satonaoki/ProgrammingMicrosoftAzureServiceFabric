using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace CalculatorService
{
    /// <summary>
    /// Service Fabric ランタイムによって、このクラスのインスタンスがサービス インスタンスごとに作成されます。
    /// </summary>
    internal sealed class CalculatorService : StatelessService, ICalculatorService
    {
        public CalculatorService(StatelessServiceContext context)
            : base(context)
        { }

        public Task<string> Add(int a, int b)
        {
            return Task.FromResult<string>(string.Format(
                "Instance {0} returns: {1}", this.Context.InstanceId, a + b));
        }
        public Task<string> Subtract(int a, int b)
        {
            return Task.FromResult<string>(string.Format(
                "Instance {0} returns: {1}", this.Context.InstanceId, a - b));
        }

        /// <summary>
        /// このサービス レプリカがクライアントやユーザーの要求を処理するために、リスナー (TCP、HTTP など) を作成するようオーバーライドします (省略可能)。
        /// </summary>
        /// <returns>リスナーのコレクション。</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context =>
                this.CreateServiceRemotingListener(context))
            };
        }
    }
}