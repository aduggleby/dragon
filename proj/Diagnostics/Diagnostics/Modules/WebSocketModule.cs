using System.Collections.Generic;
using System.Threading;
using WebSocketSharp;

namespace Dragon.Diagnostics.Modules
{
    public class WebSocketModule : DiagnosticsModuleBase<WebSocketOptions>
    {
        private const int Timeout = 60000;

        protected override void ExecuteImpl(WebSocketOptions options)
        {
            var urls = new List<string> {"ws://localhost:57703/api/test/websocket?message=short", "ws://localhost:57703/api/test/websocket?message=long"};
            urls.ForEach(TestWebsocketConnection);
        }

        private void TestWebsocketConnection(string url)
        {
            DebugMessage("Connecting to: " + url);
            using (var ws = new WebSocket(url))
            {
                var messageCount = 0;
                var expectedMessageCount = 2;
                ws.OnMessage += (sender, e) =>
                {
                    ++messageCount;
                    DebugMessage("received: " + e.Data);
                };

                ws.Connect();
                ws.Send("test");
                var timeout = Timeout;
                while (messageCount < expectedMessageCount && timeout > 0)
                {
                    Thread.Sleep(500);
                    timeout -= 500;
                }
                DebugMessage(messageCount == expectedMessageCount ? "Server responses received." : "Not all responses received!");
            }
        }
    }
}
