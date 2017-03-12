using System.Configuration;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Dragon.SecurityServer.Common;

namespace Dragon.SecurityServer.ProfileSTS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            // TODO: load from pfx for now, use certificateReference (Web.config) later
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[Consts.CertificateKey + "Name"]))
            {
                System.IdentityModel.Services.FederatedAuthentication.FederationConfigurationCreated += (sender, args) =>
                {
                    args.FederationConfiguration.IdentityConfiguration.ServiceCertificate = SecurityHelper.CreateCertificateFromConfig(Consts.CertificateKey);
                };
            }
        }
    }
}
