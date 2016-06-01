using System;
using System.Configuration;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Stores;
using Dragon.SecurityServer.PermissionSTS.Models;
using Microsoft.Owin.Security;

namespace Dragon.SecurityServer.PermissionSTS.Controllers
{
    public class HomeController : Controller
    {
        public const string Action = "wa";
        public const string SignIn = "wsignin1.0";
        public const string SignOut = "wsignout1.0";
        public const string Reply = "wreply";

        private readonly IDragonUserStore<AppMember> _userStore;

        public HomeController(IDragonUserStore<AppMember> userStore)
        {
            _userStore = userStore;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var action = Request.QueryString[Action];

                switch (action)
                {
                    case SignIn:
                        var formData = ProcessSignIn(Request.Url, (ClaimsPrincipal) User);
                        return new ContentResult() {Content = formData, ContentType = "text/html"};
                    case SignOut:
                        ProcessSignOut(Request.QueryString[Reply]);
                        break;
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
                    ProcessSignOut(Request.QueryString[Reply]);
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
            var signingCredentials = SecurityHelper.CreateSignupCredentialsFromConfig();
            var config = new SecurityTokenServiceConfiguration(ConfigurationManager.AppSettings["SecurityTokenServiceEndpointUrl"], signingCredentials);
            var sts = new CustomSecurityTokenService<AppMember>(WebConfigurationManager.AppSettings["LoginProviderName"], config, _userStore);
            var responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, user, sts);
            return responseMessage.WriteFormPost();
        }

        private static void ProcessSignOut(string redirectUri)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut(properties);
        }
    }
}