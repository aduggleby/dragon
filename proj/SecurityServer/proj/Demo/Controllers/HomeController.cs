using System;
using System.IdentityModel.Services;
using System.Web.Mvc;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.GenericSTSClient;
using WebGrease.Css.Extensions;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HmacHelper HmacHelper = new HmacHelper { HmacService = new HmacSha256Service() };

        public ActionResult Index()
        {
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

        public ActionResult SignIn()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            fam.SignIn(Guid.NewGuid().ToString());
            var signInRequestMessage = new SignInRequestMessage(new Uri(fam.Issuer), fam.Realm, Url.Action("Index", "Home", new { }, Request.Url.Scheme));
            var parameters = HmacHelper.CreateHmacRequestParametersFromConfig();
            parameters.ForEach(signInRequestMessage.Parameters.Add);
            return new RedirectResult(signInRequestMessage.WriteQueryString());
        }

        public ActionResult SignOut()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            fam.SignOut(false);
            var signOutRequestMessage = new SignOutRequestMessage(new Uri(fam.Issuer), Url.Action("Index", "Home", new { }, Request.Url.Scheme));
            var parameters = HmacHelper.CreateHmacRequestParametersFromConfig();
            parameters.ForEach(signOutRequestMessage.Parameters.Add);
            return new RedirectResult(signOutRequestMessage.WriteQueryString());
        }
    }
}