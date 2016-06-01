using System.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web.Hosting;

namespace Dragon.SecurityServer.Common
{
    public class SecurityHelper
    {
        public static X509SigningCredentials CreateSignupCredentialsFromConfig()
        {
            var certificateFilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, ConfigurationManager.AppSettings["SigningCertificateName"]);
            var data = File.ReadAllBytes(certificateFilePath);
            var certificate = new X509Certificate2(data, string.Empty, X509KeyStorageFlags.MachineKeySet);
            return new X509SigningCredentials(certificate);
        }
    }
}
