using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;

namespace CalculatorService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class CalculatorService : StatelessService, ICalculatorService
    {
        public Task<string> Add(int a, int b)
        {
            return Task.FromResult<string>(string.Format("Instance {0} returns: {1}",
                this.ServiceInitializationParameters.InstanceId,
                a + b));
        }
        public Task<string> Subtract(int a, int b)
        {
            return Task.FromResult<string>(string.Format("Instance {0} returns: {1}",
                this.ServiceInitializationParameters.InstanceId,
                a - b));
        }

protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
{
    return new[]
    {    
    new ServiceInstanceListener(initParams =>
        new WcfCommunicationListener(initParams, typeof (ICalculatorService), this)
        {
            EndpointResourceName = "ServiceEndpoint",
            Binding = this.CreateListenBinding()
        })
    };
}
        private NetTcpBinding CreateListenBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxConnections = int.MaxValue,
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize
       = Environment.ProcessorCount * binding.MaxReceivedMessageSize;

            return binding;
        }
    }
}
