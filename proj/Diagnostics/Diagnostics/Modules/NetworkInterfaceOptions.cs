using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class NetworkInterfaceOptions : DiagnosticsOptionsBase
    {
        [Option('n', "networkinterface",
            HelpText = "DISABLE: Analyzes network interfaces.")]
        public override bool Disabled { get; set; }
    }
}
