using System.Security.Claims;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Client;

namespace Dragon.SecurityServer.Demo.Controllers
{
    public class RestrictedController : Controller
    {
        private readonly IClient _client;

        public RestrictedController()
        {
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
            ViewBag.Claims = ((ClaimsIdentity) User.Identity).Claims;
            return View();
        }

        private void CustomSignIn()
        {
            System.Web.HttpContext.Current.Response.Redirect(_client.GetManagementUrl("connect", System.Web.HttpContext.Current.Request.Url.AbsoluteUri), false);
            System.Web.HttpContext.Current.Response.End();
        }
    }
}