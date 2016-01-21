using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Division42.NetworkTools.Extensions;

namespace Dragon.Diagnostics
{
    public abstract class DiagnosticsOptionsBase
    {
        [Option('h', "host", DefaultValue = "diagnostics.whataventure.com",
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
