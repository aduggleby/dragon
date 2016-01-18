using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class TraceRouteOptions : DiagnosticsOptionsBase
    {
        [Option('t', "traceroute",
            HelpText = "DISABLE: Performas a traceroute to [host].")]
        public override bool Disabled { get; set; }
    }
}
