using System.Configuration;
using System.IdentityModel.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Dragon.SecurityServer.Common;

namespace Dragon.SecurityServer.Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string CertificateKey = "Dragon.SecurityServer.EncryptingCertificate";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            // TODO: load from pfx for now, use certificateReference (Web.config) later
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[CertificateKey + "Name"]))
            {
                System.IdentityModel.Services.FederatedAuthentication.FederationConfigurationCreated += (sender, args) =>
                {
                    args.FederationConfiguration.IdentityConfiguration.ServiceCertificate = SecurityHelper.CreateCertificateFromConfig(CertificateKey);
                };
            }
        }
    }
}
