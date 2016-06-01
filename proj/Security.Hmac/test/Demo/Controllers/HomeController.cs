using System;
using System.Web;
using System.Web.Mvc;
using Dragon.Security.Hmac.Core.Service;

namespace Dragon.Security.Hmac.Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["id"] = "someContentID";
            queryString["appid"] = "00000001-0001-0001-0003-000000000001";
            queryString["serviceid"] = "00000001-0001-0001-0001-000000000001";
            queryString["userid"] = "00000001-0002-0001-0002-000000000001";
            queryString["expiry"] = DateTime.UtcNow.AddDays(+1).Ticks.ToString();
            var hmacService = new HmacSha256Service();
            queryString["signature"] = hmacService.CalculateHash(hmacService.CreateSortedQueryString(queryString), "secret");
            ViewBag.QueryString = queryString.ToString();
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
    }
}