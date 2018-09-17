using System;
using System.Diagnostics;
using System.Threading;
using Eshopworld.Telemetry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.ServiceFabric.Services.Runtime;

namespace WebAPIService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                if (IsSf)
                {
                    // The ServiceManifest.XML file defines one or more service type names.
                    // Registering a service maps a service type name to a .NET type.
                    // When Service Fabric creates an instance of this service type,
                    // an instance of the class is created in this host process.

                    ServiceRuntime.RegisterServiceAsync("greenfieldType",
                        context => new WebApiService(context)).GetAwaiter().GetResult();

                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(WebApiService).Name);

                    // Prevents this host process from terminating so services keeps running. 
                    Thread.Sleep(Timeout.Infinite);
                }
                else
                {
                    var host = WebHost.CreateDefaultBuilder()
                        .UseStartup<Startup>()
                        .Build();

                    host.Run();
                }
            }
            catch (Exception e)
            {
                BigBrother.Write(e);
                throw;
            }
        }

        /// <summary>
        /// Determines if you are running in Service Fabric or not
        /// </summary>
        private static bool IsSf
        {
            get
            {
                var sfAppName = Environment.GetEnvironmentVariable("Fabric_ApplicationName");
                var isSf = sfAppName != null;
                return isSf;
            }
        }
    }
}
