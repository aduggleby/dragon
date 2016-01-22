using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace Dragon.Diagnostics.Modules
{
    public class HttpModule : DiagnosticsModuleBase<HttpOptions>
    {
        protected override void ExecuteImpl(HttpOptions options)
        {
            var urls = new List<string>
            {
                string.Format("http://{0}/api/test/fastget", options.Host),
                string.Format("http://{0}/api/test/mediumget", options.Host),
                string.Format("http://{0}/api/test/slowget", options.Host)
            };
            var timeouts = new List<int> { 1, 10, 30 };
            for (var i = 0; i < urls.Count; ++i)
            {
                TestWebsocketConnection(urls[i], timeouts[i]);
            }
        }

        private void TestWebsocketConnection(string url, int timeout)
        {
            DebugMessage(string.Format("{0} - Connecting to: {1}", GetCurrentTime(), url));
            DebugMessage(string.Format("Wait for {0} seconds...", timeout));
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadString(url);
                    DebugMessage(string.Format("{0} - success.", GetCurrentTime()));
                }
                catch (Exception e)
                {
                    DebugMessage(string.Format("{0} - failure: {1}", GetCurrentTime(), e.Message));
                }
            }
        }

        #region helpers

        private static string GetCurrentTime()
        {
            return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
