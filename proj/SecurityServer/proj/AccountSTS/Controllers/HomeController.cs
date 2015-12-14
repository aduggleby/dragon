using System;
using System.Configuration;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Common;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.AccountSTS.Services;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Stores;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    public class HomeController : Controller
    {
        public const string Action = "wa";
        public const string Reply = "wreply";
        public const string SignIn = "wsignin1.0";
        public const string SignOut = "wsignout1.0";

        private readonly IDragonUserStore<AppMember> _userStore;

        public HomeController(IDragonUserStore<AppMember> userStore)
        {
            _userStore = userStore;
        }

        public ActionResult Index()
        {
            // TODO: handle other actions: disconnect, refresh, logout, send password
            if (User.Identity.IsAuthenticated)
            {
                switch (Request.QueryString["action"])
                {
                    case "connect":
                        // just show the login page for now
                        return new HttpUnauthorizedResult();
                    case "disconnect":
                        return RedirectToAction("ManageLogins", "Manage");
                    case "forgotPassword":
                        return RedirectToAction("ForgotPassword", "Account");
                    default:
                        // nothing to be done
                        break;
                }

                var action = Request.QueryString[Action];

                switch (action)
                {
                    case SignIn:
                        var formData = ProcessSignIn(Request.Url, (ClaimsPrincipal) User);
                        return new ContentResult() {Content = formData, ContentType = "text/html"};
                    case SignOut:
                        ProcessSignOut();
                        return Redirect(Request.QueryString[Reply]);
                    default:
                        // nothing to be done
                        break;
                }
            }
            else
            {
                // If the user just wants to sign out, do so...
                if (Request.QueryString[Action] == SignOut)
                {
                    ProcessSignOut();
                    return Redirect(Request.QueryString[Reply]);
                }
                // ... else show login page
                return new HttpUnauthorizedResult();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private string ProcessSignIn(Uri url, ClaimsPrincipal user)
        {
            var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(url);
            var config = new SecurityTokenServiceConfiguration(ConfigurationManager.AppSettings["IssuerName"], SecurityHelper.CreateSignupCredentialsFromConfig());
            var sts = new CustomSecurityTokenService(config, _userStore);
            var responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, user, sts);
            return responseMessage.WriteFormPost();
        }

        private static void ProcessSignOut()
        {
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut();
        }
    }
}