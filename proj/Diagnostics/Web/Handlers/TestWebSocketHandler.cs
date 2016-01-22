using System.Threading;
using Microsoft.Web.WebSockets;

namespace Dragon.Diagnostics.Web.Handlers
{
    public class TestWebSocketHandler : WebSocketHandler
    {
        private readonly string _message;
        private static readonly WebSocketCollection ChatClients = new WebSocketCollection();

        public TestWebSocketHandler(string message)
        {
            _message = message;
        }

        public override void OnOpen()
        {
            ChatClients.Add(this);
            Broadcast(_message);
        }

        private void Broadcast(string message)
        {
            var timeOut = _message == "long" ? 30000 : _message == "medium" ? 10000 : _message == "short" ? 1000 : 0;
            Thread.Sleep(timeOut);
            ChatClients.Broadcast(message);
        }

        public override void OnMessage(string message)
        {
            Broadcast(message);
        }
    }
}