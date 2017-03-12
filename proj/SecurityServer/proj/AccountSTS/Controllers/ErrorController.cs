using System.Web.Mvc;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Unauthorized()
        {
            return View();
        }
    }
}