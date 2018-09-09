using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dragon.Data.Interfaces;
using Dragon.SecurityServer.AccountSTS.Attributes;
using Dragon.SecurityServer.AccountSTS.Helpers;
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
        private readonly IRepository<UserActivity> _userActivityRepository;
        private readonly IProviderLimiterService _providerLimiterService;

        public HomeController(IDragonUserStore<AppMember> userStore, IFederationService federationService, ApplicationSignInManager signInManager, ApplicationUserManager userManager, IRepository<UserActivity> userActivityRepository, IProviderLimiterService providerLimiterService)
        {
            _userStore = userStore;
            _federationService = federationService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userActivityRepository = userActivityRepository;
            _providerLimiterService = providerLimiterService;
        }

        [AuthorizeForRegisteredApps]
        [ProviderAware]
        public async Task<ActionResult> Index()
        {
            Debug.Assert(Request.Url != null, "Request.Url != null");
            var routeValues = new Dictionary<string, object> // Use <string, object> to call the correct RedirectToAction overload
            {
                {"returnUrl", Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)},
            };
            Consts.QueryStringHmacParameterNames.ForEach(x => routeValues.Add(x, HttpContext.Request.QueryString[x]));

            // TODO: handle other actions: disconnect, refresh, logout, send password
            if (User.Identity.IsAuthenticated)
            {
                switch (Request.QueryString["action"])
                {
                    case "reconnect":
                        var formData = ProcessSignIn(Request.Url, (ClaimsPrincipal)User);
                        return new ContentResult() { Content = formData, ContentType = "text/html" };
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
                    case "manage":
                        return RedirectToAction("Index", "Manage");
                    case "admin-users":
                        return RedirectToAction("Users", "Admin");
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
                switch (Request.QueryString["action"])
                {
                    case "register":
                        return RedirectToAction("Register", "Account", new RouteValueDictionary(routeValues));
                }
                // If the user just wants to sign out, do so...
                if (Request.QueryString[Action] == SignOut)
                {
                    ProcessSignOut();
                    return Redirect(Request.QueryString[Reply]);
                }
                // ... else show login page
                // return new HttpUnauthorizedResult(); // we need the hmac parameters, so use a custom redirect
                Debug.Assert(HttpContext.Request.Url != null, "HttpContext.Request.Url != null");
                
                var routeValuesDictionary = routeValues.Where(x => x.Value != null).ToDictionary(k => k.Key, k => k.Value.ToString());
                var providerToUse = _providerLimiterService.Select(routeValuesDictionary);
                if (!string.IsNullOrWhiteSpace(providerToUse))
                {
                    var routeValueDictionary = new RouteValueDictionary(_federationService.CreateRouteValues(
                        routeValuesDictionary["returnUrl"],
                        DictionaryToNameValueCollection(routeValuesDictionary),
                        HttpContext.Request.QueryString["wreply"]));
                    return _federationService.PerformExternalLogin(ControllerContext.HttpContext, providerToUse,
                        Url.Action("ExternalLoginCallback", "Account", routeValueDictionary));
                }

                return RedirectToAction("Login", "Account", new RouteValueDictionary(routeValues));
            }
            return View();
        }

        private static NameValueCollection DictionaryToNameValueCollection(Dictionary<string, string> routeValues)
        {
            var routeValuesCollection = routeValues.Aggregate(new NameValueCollection(),
                (seed, current) =>
                {
                    seed.Add(current.Key, (string) current.Value);
                    return seed;
                });
            return routeValuesCollection;
        }

        [ProviderRestriction]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ProviderRestriction]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private string ProcessSignIn(Uri url, ClaimsPrincipal user)
        {
            var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(url);
            var config = new SecurityTokenServiceConfiguration(ConfigurationManager.AppSettings["IssuerName"], SecurityHelper.CreateSignupCredentialsFromConfig());
            var encryptionCredentials = SecurityHelper.CreateEncryptingCredentialsFromConfig();
            var sts = new CustomSecurityTokenService(config, encryptionCredentials, _userStore);
            var responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, user, sts);
            return responseMessage.WriteFormPost();
        }

        private void ProcessSignOut()
        {
            _userActivityRepository.Insert(new UserActivity
            {
                AppId = RequestHelper.GetCurrentAppId() ?? Guid.Empty.ToString(),
                ServiceId = RequestHelper.GetCurrentServiceId() ?? Guid.Empty.ToString(),
                DateTime = DateTime.UtcNow,
                Type = "Logout",
                UserId = User.Identity.GetUserId() ?? Guid.Empty.ToString(),
                Details = ""
            });
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            System.Web.HttpContext.Current.Session.Abandon();
        }
    }
}