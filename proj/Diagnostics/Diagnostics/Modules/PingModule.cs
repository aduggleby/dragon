using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Dragon.Diagnostics.Modules
{
    public class PingModule : DiagnosticsModuleBase<PingOptions>
    {
        protected override void ExecuteImpl(PingOptions options)
        {
            var hostsToCheck = options.PingHost.Split(',').Select(x => x.Trim());
            
            foreach (var host in hostsToCheck)
            {
                DebugMessage(string.Format("Pinging {0}...", host));
                var success = IsPingable(host);
                DebugMessage(success ? "Success!" : "Failure.");
            }
        }

        private bool IsPingable(string host)
        {
            var pingable = false;
            var pinger = new Ping();
            try
            {
                var reply = pinger.Send(host);
                pingable = reply != null && reply.Status == IPStatus.Success;
            }
            catch (PingException e)
            {
                DebugMessage(e.Message + Environment.NewLine + e.InnerException.Message);
            }
            return pingable;
        }
    }
}
