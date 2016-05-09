using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.AccountSTS.Services;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    public class HomeController : Controller
    {
        public const string Action = "wa";
        public const string Reply = "wreply";
        public const string SignIn = "wsignin1.0";
        public const string SignOut = "wsignout1.0";

        private readonly IDragonUserStore<AppMember> _userStore;
        private readonly IFederationService _federationService;
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;

        public HomeController(IDragonUserStore<AppMember> userStore, IFederationService federationService, ApplicationSignInManager signInManager, ApplicationUserManager userManager)
        {
            _userStore = userStore;
            _federationService = federationService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            Debug.Assert(Request.Url != null, "Request.Url != null");
            var routeValues = new Dictionary<string, object>
            {
                {"returnUrl", Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)},
            };
            Consts.QueryStringHmacParameterNames.ForEach(x => routeValues.Add(x, HttpContext.Request.QueryString[x]));

            // TODO: handle other actions: disconnect, refresh, logout, send password
            if (User.Identity.IsAuthenticated)
            {
                switch (Request.QueryString["action"])
                {
                    case "connect":
                        routeValues.Remove("returnUrl");
                        routeValues.Add("returnUrl", Request.QueryString[Reply]);
                        return _federationService.PerformExternalLogin(
                            ControllerContext.HttpContext,
                            Request.QueryString["data"],
                            Url.Action("ExternalLoginCallbackAddLogin", "Account", new RouteValueDictionary(routeValues)));
                    case "disconnect":
                        return await _federationService.Disconnect(
                            _signInManager, 
                            _userManager, 
                            Request.QueryString["data"], 
                            User.Identity.GetUserId(), 
                            Request.QueryString[Reply]);
                    case "forgotPassword":
                        return RedirectToAction("ForgotPassword", "Account", new RouteValueDictionary(routeValues));
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
                // return new HttpUnauthorizedResult(); // we need the hmac parameters, so use a custom redirect
                Debug.Assert(HttpContext.Request.Url != null, "HttpContext.Request.Url != null");
                return RedirectToAction("Login", "Account", new RouteValueDictionary(routeValues));
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