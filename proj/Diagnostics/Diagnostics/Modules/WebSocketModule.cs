using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using WebSocketSharp;

namespace Dragon.Diagnostics.Modules
{
    public class WebSocketModule : DiagnosticsModuleBase<WebSocketOptions>
    {
        protected override void ExecuteImpl(WebSocketOptions options)
        {
            var urls = new List<string>
            {
                string.Format("ws://{0}/api/test/websocket?message=short", options.Host), 
                string.Format("ws://{0}/api/test/websocket?message=medium", options.Host), 
                string.Format("ws://{0}/api/test/websocket?message=long", options.Host)
            };
            var timeouts = new List<int> {2, 20, 60};
            for (var i = 0; i < urls.Count; ++i)
            {
                TestWebsocketConnection(urls[i], timeouts[i]);
            }
        }

        private void TestWebsocketConnection(string url, int timeout)
        {
            DebugMessage(string.Format("{0} - Connecting to: {1}", GetCurrentTime(), url));
            DebugMessage(string.Format("Waiting for {0} seconds...", timeout));
            using (var ws = new WebSocket(url))
            {
                var messageCount = 0;
                var expectedMessageCount = 2;
                ws.OnMessage += (sender, e) =>
                {
                    ++messageCount;
                    DebugMessage(string.Format("{0} - received: {1}", GetCurrentTime(), e.Data));
                };

                ws.Connect();
                ws.Send("test");
                var waitedForMs = 0; 
                while (messageCount < expectedMessageCount && waitedForMs < timeout * 1000 + 5 * 1000) // in ms, add a small buffer to the expected timeout
                {
                    const int sleepTime = 500;
                    Thread.Sleep(sleepTime);
                    waitedForMs += sleepTime;
                }
                DebugMessage(string.Format("{0} - Done", GetCurrentTime()));
                DebugMessage(messageCount == expectedMessageCount ? "Server responses received." : "Not all responses received!");
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
