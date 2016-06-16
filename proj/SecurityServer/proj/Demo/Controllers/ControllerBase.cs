using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Client;
using System.Linq;
using System.Web.Routing;
using Dragon.SecurityServer.GenericSTSClient;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected readonly IClient _client;

        protected ControllerBase()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            var accountStsUrl = ConfigurationManager.AppSettings["AccountStsUrl"];
            accountStsUrl = (string.IsNullOrEmpty(accountStsUrl) ? fam.Issuer : accountStsUrl);
            _client = new AccountSTSClient(accountStsUrl + "/Api", accountStsUrl, fam.Realm);
            _client.SetHmacSettings(HmacHelper.ReadHmacSettings());
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