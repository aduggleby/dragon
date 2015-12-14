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
            return new X509SigningCredentials(new X509Certificate2(X509Certificate.CreateFromCertFile(
                    Path.Combine(HostingEnvironment.ApplicationPhysicalPath, ConfigurationManager.AppSettings["SigningCertificateName"]))));
        }
    }
}
