namespace WebCalculatorService
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Fabric;
    using Microsoft.Owin.Hosting;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using System;

    internal class OwinCommunicationListener : ICommunicationListener
    {
        private readonly IOwinAppBuilder startup;
        private readonly ServiceContext serviceContext;
        private readonly ServiceEventSource eventSource;
        private IDisposable serverHandle;
        private string listeningAddress;

        public OwinCommunicationListener(IOwinAppBuilder startup,
        ServiceContext serviceContext, ServiceEventSource eventSource)
        {
            this.startup = startup;
            this.serviceContext = serviceContext;
            this.eventSource = eventSource;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var serviceEndpoint =
            this.serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = serviceEndpoint.Port;
            this.listeningAddress = string.Format("http://+:{0}/", port);
            this.serverHandle = WebApp.Start(
                this.listeningAddress,
                appBuilder => this.startup.Configuration(appBuilder));
                string resultAddress = this.listeningAddress.Replace(
                    "+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
            this.eventSource.Message("Listening on {0}", resultAddress);
            return Task.FromResult(resultAddress);
        }
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            this.StopWebServer();
            return Task.FromResult(true);
        }
        public void Abort()
        {
            this.StopWebServer();
        }
        private void StopWebServer()
        {
            if (this.serverHandle != null)
            {
                try
                {
                    this.serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // NO-OP
                }
            }
        }
    }
}