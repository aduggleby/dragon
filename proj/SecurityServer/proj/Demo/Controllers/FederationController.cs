using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.GenericSTSClient;
using WebGrease.Css.Extensions;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public class FederationController : Controller
    {
        private static readonly HmacHelper HmacHelper = new HmacHelper { HmacService = new HmacSha256Service() };

        public ActionResult OnExternalFederationChanged(string returnUrl)
        {
            return SignOut(GetReturnUrl(returnUrl, "OnFederationLogout"));
        }

        public ActionResult OnFederationLogout(string returnUrl)
        {
            return SignIn(GetReturnUrl(returnUrl, "OnFederationLogin"));
        }

        public ActionResult OnFederationLogin(string returnUrl)
        {
            return Redirect(returnUrl);
        }

        public ActionResult SignIn(string returnUrl)
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            fam.SignIn(Guid.NewGuid().ToString());
            var signInRequestMessage = new SignInRequestMessage(new Uri(fam.Issuer), fam.Realm, returnUrl);
            var parameters = HmacHelper.CreateHmacRequestParametersFromConfig();
            parameters.ForEach(signInRequestMessage.Parameters.Add);
            return new RedirectResult(signInRequestMessage.WriteQueryString());
        }

        public ActionResult SignOut(string returnUrl)
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            fam.SignOut(false);
            FormsAuthentication.SignOut();
            Debug.Assert(Request.Url != null, "Request.Url != null");
            var signOutRequestMessage = new SignOutRequestMessage(new Uri(fam.Issuer), returnUrl);
            var parameters = HmacHelper.CreateHmacRequestParametersFromConfig();
            parameters.ForEach(signOutRequestMessage.Parameters.Add);
            return new RedirectResult(signOutRequestMessage.WriteQueryString());
        }

        private string GetReturnUrl(string returnUrl, string actionName)
        {
            Debug.Assert(Request.Url != null, "Request.Url != null");
            var routeValues = new Dictionary<string, object> { { "returnUrl", returnUrl } };
            var url = Url.Action(actionName, "Federation", new RouteValueDictionary(routeValues), Request.Url.Scheme);
            return url;
        }
    }
}