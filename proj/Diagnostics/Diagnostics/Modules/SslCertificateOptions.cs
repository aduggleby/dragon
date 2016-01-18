using CommandLine;

namespace Dragon.Diagnostics.Modules
{
    public class SslCertificateOptions : DiagnosticsOptionsBase
    {
        [Option('s', "sslcertificate",
            HelpText = "DISABLE: Checks a SSL certificate.")]
        public override bool Disabled { get; set; }
    }
}
