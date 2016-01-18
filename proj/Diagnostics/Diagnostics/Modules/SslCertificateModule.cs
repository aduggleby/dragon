using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Dragon.Diagnostics.Modules
{
    public class SslCertificateModule : DiagnosticsModuleBase<SslCertificateOptions>
    {
        private bool _certificateFound;
        
        protected override void ExecuteImpl(SslCertificateOptions options)
        {
            var requestUriString = "https://" + options.Host;
            _certificateFound = false;
            DebugMessage("Connecting to: " + requestUriString);
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
            var request = WebRequest.Create(requestUriString);
            request.GetResponse();
            if (!_certificateFound)
            {
                DebugMessage("No certificate has been found!");
            }
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            DebugMessage(certificate.ToString());
            _certificateFound = true;
            return true;
        }
    }
}
