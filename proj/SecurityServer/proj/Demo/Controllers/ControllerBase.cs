using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Client;
using System.Linq;
using System.Web.Routing;
using Dragon.SecurityServer.GenericSTSClient;
using Dragon.SecurityServer.GenericSTSClient.Models;
using Dragon.SecurityServer.ProfileSTS.Client;
using Microsoft.AspNet.Identity;
using IClient = Dragon.SecurityServer.AccountSTS.Client.IClient;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected IClient Client;
        protected ProfileSTS.Client.IClient ProfileClient;

        protected ControllerBase()
        {
        }

        protected ControllerBase(IClient client)
        {
            Client = client;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            InitClients();
            InitViewBag();
        }

        private void InitViewBag()
        {
            ViewBag.Name = User.Identity.Name;
            ViewBag.AuthenticationType = User.Identity.AuthenticationType;
            var claims = ((ClaimsIdentity) User.Identity).Claims.ToList();
            ViewBag.Claims = claims;
            Debug.Assert(Request.Url != null, "Request.Url != null");
            ViewBag.ManageUrl = Client.GetFederationUrl("manage", Request.Url.ToString());
        }

        private void InitClients()
        {
            if (Client == null)
            {
                InitAccountClient();
            }
            if (ProfileClient == null)
            {
                InitProfileClient();
            }
        }

        private void InitProfileClient()
        {
            var profileStsUrl = ConfigurationManager.AppSettings["ProfileStsUrl"];
            if (string.IsNullOrEmpty(profileStsUrl))
            {
                throw new ConfigurationErrorsException("App setting 'ProfileStsUrl' is missing.");
            }
            ProfileClient = new ProfileSTSClient(profileStsUrl + "/Api");
            ProfileClient.SetHmacSettings(ReadHmacSettings(Consts.ProfileHmacSettingsPrefix));
        }

        private void InitAccountClient()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            var accountStsUrl = ConfigurationManager.AppSettings["AccountStsUrl"];
            accountStsUrl = (string.IsNullOrEmpty(accountStsUrl) ? fam.Issuer : accountStsUrl);
            Client = new AccountSTSClient(accountStsUrl + "/Api", accountStsUrl, fam.Realm);
            Client.SetHmacSettings(ReadHmacSettings(Consts.AccountHmacSettingsPrefix));
        }

        private HmacSettings ReadHmacSettings(string hmacSettingsPrefix)
        {
            return HmacHelper.ReadHmacSettings(hmacSettingsPrefix, (User?.Identity?.IsAuthenticated ?? false) ? User.Identity.GetUserId() : null);
        }
    }
}