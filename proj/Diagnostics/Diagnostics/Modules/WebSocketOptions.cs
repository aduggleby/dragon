using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class WebSocketOptions : DiagnosticsOptionsBase
    {
        [Option('w', "websockets",
            HelpText = "DISABLE: Checks websocket connectivity.")]
        public override bool Disabled { get; set; }
    }
}
