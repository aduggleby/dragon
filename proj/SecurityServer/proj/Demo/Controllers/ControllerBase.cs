using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Client;
using System.Linq;
using System.Web.Routing;
using Dragon.SecurityServer.GenericSTSClient;
using Dragon.SecurityServer.ProfileSTS.Client;
using IClient = Dragon.SecurityServer.AccountSTS.Client.IClient;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected readonly IClient _client;
        protected readonly ProfileSTS.Client.IClient _profileClient;

        protected ControllerBase()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            var accountStsUrl = ConfigurationManager.AppSettings["AccountStsUrl"];
            accountStsUrl = (string.IsNullOrEmpty(accountStsUrl) ? fam.Issuer : accountStsUrl);
            _client = new AccountSTSClient(accountStsUrl + "/Api", accountStsUrl, fam.Realm);
            _client.SetHmacSettings(HmacHelper.ReadHmacSettings());
            var profileStsUrl = ConfigurationManager.AppSettings["ProfileStsUrl"];
            if (string.IsNullOrEmpty(profileStsUrl))
            {
                throw new ConfigurationErrorsException("App setting 'ProfileStsUrl' is missing.");
            }
            _profileClient = new ProfileSTSClient(profileStsUrl + "/Api");
            _profileClient.SetHmacSettings(HmacHelper.ReadHmacSettings());
        }

        protected ControllerBase(IClient client)
        {
            _client = client;
        }

        private void InitViewBag()
        {
            ViewBag.Name = User.Identity.Name;
            ViewBag.AuthenticationType = User.Identity.AuthenticationType;
            var claims = ((ClaimsIdentity) User.Identity).Claims.ToList();
            ViewBag.Claims = claims;
            Debug.Assert(Request.Url != null, "Request.Url != null");
            ViewBag.ManageUrl = _client.GetFederationUrl("manage", Request.Url.ToString());
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            InitViewBag();
        }
    }
}