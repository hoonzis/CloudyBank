using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure;
using System.Diagnostics;

namespace CloudyBank.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            try
            {
                DiagnosticMonitorConfiguration diagConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
                //diagConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
                diagConfig.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
                diagConfig.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

                DiagnosticMonitor.Start("DataConnectionString", diagConfig);

                // For information on handling configuration changes
                // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
                RoleEnvironment.Changing += RoleEnvironmentChanging;

                // This code sets up a handler to update CloudStorageAccount instances when their corresponding
                // configuration settings change in the service configuration file.

                // This code sets up a handler to update CloudStorageAccount instances when their corresponding
                // configuration settings change in the service configuration file.
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    // Provide the configSetter with the initial value
                    configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

                    RoleEnvironment.Changed += (s, arg) =>
                    {
                        if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>()
                          .Any((change) => (change.ConfigurationSettingName == configName)))
                        {
                            // The corresponding configuration setting has changed, propagate the value
                            if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
                            {
                                // In this case, the change to the storage account credentials in the
                                // service configuration is significant enough that the role needs to be
                                // recycled in order to use the latest settings. (for example, the 
                                // endpoint has changed)
                                RoleEnvironment.RequestRecycle();
                            }
                        }
                    };
                });

                var azureTraceListener = new Microsoft.WindowsAzure.Diagnostics.DiagnosticMonitorTraceListener();

                Trace.Listeners.Add(azureTraceListener);
            }
            catch (Exception)
            {

            }
            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
