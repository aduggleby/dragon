using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class PingOptions : DiagnosticsOptionsBase
    {
        [Option('p', "ping",
            HelpText = "DISABLE: Checks for basic HTTP/HTTPS connectivity.")]
        public override bool Disabled { get; set; }

        [Option('i', "pinghosts", DefaultValue = "8.8.8.8, example.org",
            HelpText = "The webservice to test against.")]
        public string PingHost { get; set; }
    }
}
