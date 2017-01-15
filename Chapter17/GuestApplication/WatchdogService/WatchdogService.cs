using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WatchdogService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
internal sealed class WatchdogService : StatelessService
{
    private Uri applicationName = new Uri("fabric:/TimeServiceApplication");
    private string serviceManifestName = "TimeServicePkg";
    private string nodeName = FabricRuntime.GetNodeContext().NodeName;
    private FabricClient Client = new FabricClient(new FabricClientSettings() { HealthReportSendInterval = TimeSpan.FromSeconds(0) });

    protected override async Task RunAsync(CancellationToken cancelServiceInstance)
    {
           
        int failedCount = 0;
        while (!cancelServiceInstance.IsCancellationRequested)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string payload = client.DownloadString(new Uri("http://localhost:8088/"));
                    if (!string.IsNullOrEmpty(payload))
                    {
                        failedCount = 0;
                        var deployedServicePackageHealthReport = new DeployedServicePackageHealthReport(
                                applicationName,
                                serviceManifestName,
                                nodeName,
                                new HealthInformation("CustomWatchDog", "WebServerHealth", HealthState.Ok));
                        Client.HealthManager.ReportHealth(deployedServicePackageHealthReport);
                        ServiceEventSource.Current.ServiceMessage(this, "Watchdog is happy");
                    }
                }
                catch (WebException)
                {
                    failedCount++;
                    ServiceEventSource.Current.ServiceMessage(this, "Watchdog had detected " + failedCount + " failures.");
                    if (failedCount >= 5)
                    {
                        var deployedServicePackageHealthReport = new DeployedServicePackageHealthReport(
                            applicationName,
                            serviceManifestName,
                            nodeName,
                            new HealthInformation("CustomWatchDog", "WebServerHealth", HealthState.Error));
                        Client.HealthManager.ReportHealth(deployedServicePackageHealthReport);
                        ServiceEventSource.Current.ServiceMessage(this, "Watchdog is sad.");
                    }
                }
            }
                    
            await Task.Delay(TimeSpan.FromSeconds(1), cancelServiceInstance);
        }
    }
}
}
