using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Dragon.Diagnostics.Modules
{
    public class NetworkInterfaceModule : DiagnosticsModuleBase<NetworkInterfaceOptions>
    {
        protected override void ExecuteImpl(NetworkInterfaceOptions options)
        {
            foreach (var ifs in NetworkInterface.GetAllNetworkInterfaces())
            {
                DebugMessage(ifs.Name);
            }

            var proxy = WebRequest.DefaultWebProxy.GetProxy(new Uri("http://" + options.Host));
            if (proxy.Host != "0.0.0.1")
            {
                DebugMessage(proxy.OriginalString);
            }
        }
    }
}
