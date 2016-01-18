using CommandLine;
using CommandLine.Text;

namespace Dragon.Diagnostics
{
    public abstract class DiagnosticsOptionsBase
    {
        [Option('v', "verbose", DefaultValue = true,
            HelpText = "Prints debug messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('h', "host", DefaultValue = "my.whataventure.com",
            HelpText = "The webservice to test against.")]
        public string Host { get; set; }

        public virtual bool Disabled { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
