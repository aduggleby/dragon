using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Dragon.Diagnostics.Modules
{
    public class NetworkInterfaceModule : DiagnosticsModuleBase<NetworkInterfaceOptions>
    {
        protected override void ExecuteImpl(NetworkInterfaceOptions options)
        {
            DebugMessage("Available network interfaces:");
            foreach (var ifs in NetworkInterface.GetAllNetworkInterfaces())
            {
                DebugMessage(string.Format("- {0}: {1}", ifs.Name, ifs.OperationalStatus));
            }
            DebugMessage("\nConfigured proxy:");
            var proxy = WebRequest.DefaultWebProxy.GetProxy(new Uri("http://" + options.Host));
            DebugMessage(proxy.Host != options.Host ? proxy.OriginalString : "none.");
        }
    }
}
