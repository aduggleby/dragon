using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class HttpOptions : DiagnosticsOptionsBase
    {
        [Option('h', "http",
            HelpText = "DISABLE: Checks http connectivity.")]
        public override bool Disabled { get; set; }
    }
}
