using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public interface IFederationService
    {
        ActionResult PerformExternalLogin(HttpContextBase context, string provider, string returnUrl);
        ActionResult PerformExternalLogin(HttpContextBase context, string provider, string returnUrl, string userId);
        Task<ActionResult> Disconnect(ApplicationSignInManager signInManager, ApplicationUserManager userManager, string provider, string userId, string redirectUri);
        Dictionary<string, object> CreateRouteValues(string returnUrl, NameValueCollection queryStringCollection, string wreply);
    }
}