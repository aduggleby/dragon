using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Client;
using System.Linq;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public class RestrictedController : Controller
    {
        private readonly IClient _client;
        public const string ManagementConnectedAccountType = "http://whataventure.com/schemas/identity/claims/account/connectedAccountType";
        public const string ManagementDisconnectedAccountType = "http://whataventure.com/schemas/identity/claims/account/disconnectedAccountType";

        public RestrictedController()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            var accountStsUrl = ConfigurationManager.AppSettings["AccountStsUrl"];
            _client = new AccountSTSClient(string.IsNullOrEmpty(accountStsUrl) ? fam.Issuer : accountStsUrl, fam.Realm);
        }

        public RestrictedController(IClient client)
        {
            _client = client;
        }

        // GET: Restricted
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
                //CustomSignIn(); // custom signin
            }
            ViewBag.Name = User.Identity.Name;
            ViewBag.AuthenticationType = User.Identity.AuthenticationType;
            var claims = ((ClaimsIdentity) User.Identity).Claims.ToList();
            ViewBag.Claims = claims;
            ViewBag.ConnectUrls = GetFederationManagementUrls(claims, ManagementDisconnectedAccountType, "connect");
            ViewBag.DisconnectUrls = GetFederationManagementUrls(claims, ManagementConnectedAccountType, "disconnect");
            return View();
        }

        private Dictionary<string, string> GetFederationManagementUrls(IEnumerable<Claim> claims, string accountType, string action)
        {
            var types = claims.Where(x => x.Type == accountType).ToList();
            return types.Any()
                ? types.ToDictionary(
                    x => x.Value,
                    x => _client.GetManagementUrl(action, x.Value, System.Web.HttpContext.Current.Request.Url.AbsoluteUri)
                    )
                : new Dictionary<string, string>();
        }

        private void CustomSignIn()
        {
            System.Web.HttpContext.Current.Response.Redirect(_client.GetManagementUrl("connect", System.Web.HttpContext.Current.Request.Url.AbsoluteUri), false);
            System.Web.HttpContext.Current.Response.End();
        }
    }
}