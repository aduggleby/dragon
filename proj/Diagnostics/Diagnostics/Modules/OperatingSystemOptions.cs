using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class OperatingSystemOptions : DiagnosticsOptionsBase
    {
        [Option('o', "operatingsystem",
            HelpText = "DISABLE: Analyzes operating system basics.")]
        public override bool Disabled { get; set; }
    }
}
