using System;
using System.IdentityModel.Services;
using System.Web.Configuration;
using System.Web.Mvc;
using Dragon.SecurityServer.Common;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public class HomeController : Controller
    {
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
            signInRequestMessage.Parameters.Add(Consts.QueryStringParameterNameServiceId, WebConfigurationManager.AppSettings["ServiceId"]);
            return new RedirectResult(signInRequestMessage.WriteQueryString());
        }

        public ActionResult SignOut()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            fam.SignOut(false);
            var signOutRequestMessage = new SignOutRequestMessage(new Uri(fam.Issuer), Url.Action("Index", "Home", new { }, Request.Url.Scheme));
            signOutRequestMessage.Parameters.Add(Consts.QueryStringParameterNameServiceId, WebConfigurationManager.AppSettings["ServiceId"]);
            return new RedirectResult(signOutRequestMessage.WriteQueryString());
        }
    }
}