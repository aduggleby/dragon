using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Client;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Dragon.SecurityServer.AccountSTS.Client.Models;
using Dragon.SecurityServer.Demo.ActionFilters;
using Dragon.SecurityServer.Demo.Models;
using Dragon.SecurityServer.GenericSTSClient;
using Microsoft.AspNet.Identity;

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
            accountStsUrl = (string.IsNullOrEmpty(accountStsUrl) ? fam.Issuer : accountStsUrl);
            _client = new AccountSTSClient(accountStsUrl + "/Api", accountStsUrl, fam.Realm);
            _client.SetHmacSettings(HmacHelper.ReadHmacSettings());
        }

        public RestrictedController(IClient client)
        {
            _client = client;
        }

        // GET: Restricted
        [ImportModelStateFromTempData]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
                //CustomSignIn(); // custom signin
            }
            InitViewBag();
            return View();
        }

        private void InitViewBag()
        {
            ViewBag.Name = User.Identity.Name;
            ViewBag.AuthenticationType = User.Identity.AuthenticationType;
            var claims = ((ClaimsIdentity) User.Identity).Claims.ToList();
            ViewBag.Claims = claims;
            ViewBag.ConnectUrls = GetFederationManagementUrls(claims, ManagementDisconnectedAccountType, "connect");
            ViewBag.DisconnectUrls = GetFederationManagementUrls(claims, ManagementConnectedAccountType, "disconnect");
        }

        private Dictionary<string, string> GetFederationManagementUrls(IEnumerable<Claim> claims, string accountType, string action)
        {
            var routeValues = new Dictionary<string, object> { { "returnUrl", System.Web.HttpContext.Current.Request.Url.AbsoluteUri } };
            var types = claims.Where(x => x.Type == accountType).ToList();
            Debug.Assert(Request.Url != null, "Request.Url != null");
            return types.Any()
                ? types.ToDictionary(
                    x => x.Value,
                    x => _client.GetFederationUrl(action, x.Value,
                        Url.Action("OnExternalFederationChanged", "Federation", new RouteValueDictionary(routeValues), Request.Url.Scheme)))
                : new Dictionary<string, string>();
        }

        private void CustomSignIn()
        {
            System.Web.HttpContext.Current.Response.Redirect(_client.GetFederationUrl("connect", System.Web.HttpContext.Current.Request.Url.AbsoluteUri), false);
            System.Web.HttpContext.Current.Response.End();
        }

        [HttpPost]
        [ExportModelStateToTempData]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeAccountData(ChangeAccountDataViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            var id = ((ClaimsIdentity)User.Identity).Claims.ToList().First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var updateViewModel = new UpdateViewModel
            {
                Id = id,
            };
            if (!string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                updateViewModel.Email = model.EmailAddress;
            }
            // TODO: validate pw
            // TODO: old pw? what if non exists?
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                updateViewModel.Password = model.NewPassword;
                updateViewModel.ConfirmPassword = model.ConfirmPassword;
            }

            try
            {
                await _client.Update(updateViewModel);

                // update claims to provide the user up-to-date data
                var identity = (ClaimsIdentity)User.Identity;
                var context = Request.GetOwinContext();
                var claim = identity.FindFirst(ClaimTypes.Email);
                if (!string.IsNullOrWhiteSpace(model.EmailAddress) && claim.Value != model.EmailAddress)
                {
                    identity.RemoveClaim(claim);
                    identity.AddClaim(new Claim(ClaimTypes.Email, updateViewModel.Email));
                }
                context.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                context.Authentication.SignIn(identity);
            }
            catch (ApiException e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return RedirectToAction("Index", model);
        }
    }
}