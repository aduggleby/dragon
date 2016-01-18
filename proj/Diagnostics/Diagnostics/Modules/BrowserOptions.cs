using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class BrowserOptions : DiagnosticsOptionsBase
    {
        [Option('b', "broser",
            HelpText = "DISABLE: Checks for installed browsers.")]
        public override bool Disabled { get; set; }
    }
}
