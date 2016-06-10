using System.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web.Hosting;

namespace Dragon.SecurityServer.Common
{
    public class SecurityHelper
    {
        private const string EncryptionCertificateName = "EncryptingCertificate";
        private const string SigningCertificateName = "SigningCertificate";

        public static X509SigningCredentials CreateSignupCredentialsFromConfig()
        {
            return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[SigningCertificateName + "Name"]) ?
                null :
                new X509SigningCredentials(CreateCertificateFromConfig(SigningCertificateName));
        }

        public static X509EncryptingCredentials CreateEncryptingCredentialsFromConfig()
        {
            return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[EncryptionCertificateName + "Name"]) ?
                null :
                new X509EncryptingCredentials(CreateCertificateFromConfig(EncryptionCertificateName));
        }

        public static X509Certificate2 CreateCertificateFromConfig(string certificateName)
        {
            var certificateFilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, ConfigurationManager.AppSettings[certificateName + "Name"]);
            var data = File.ReadAllBytes(certificateFilePath);
            var certificate = new X509Certificate2(data, ConfigurationManager.AppSettings[certificateName + "Password"], X509KeyStorageFlags.MachineKeySet);
            return certificate;
        }
    }
}
