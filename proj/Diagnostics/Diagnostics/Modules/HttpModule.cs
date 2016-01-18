using System;
using System.Collections.Generic;
using System.Net;

namespace Dragon.Diagnostics.Modules
{
    public class HttpModule : DiagnosticsModuleBase<HttpOptions>
    {
        protected override void ExecuteImpl(HttpOptions options)
        {
            var urls = new List<string> { "http://localhost:57703/api/test/fastget", "http://localhost:57703/api/test/slowget" };
            urls.ForEach(TestWebsocketConnection);
        }

        private void TestWebsocketConnection(string url)
        {
            DebugMessage("Connecting to: " + url);
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadString(url);
                    DebugMessage("success.");
                }
                catch (Exception e)
                {
                    DebugMessage("failure: " + e.Message);
                }
            }
        }
    }
}
