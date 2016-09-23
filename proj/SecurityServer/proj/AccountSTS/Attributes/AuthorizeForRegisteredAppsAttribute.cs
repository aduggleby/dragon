using System;
using System.IdentityModel.Claims;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.AccountSTS.Services;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Attributes
{
    /// <summary>
    /// Allows access if:
    /// * the request contains the userid and appid properties
    /// * the specified user is registered for the provided app, <see cref="IdentityUserApp" />
    /// </summary>
    public class AuthorizeForRegisteredAppsAttribute : AuthorizeAttribute
    {
        private const string UnauthorizedUrl = "~/Error/Unauthorized";

        private readonly IAppService _appService;

        public AuthorizeForRegisteredAppsAttribute()
        {
            _appService = new AppService(new Repository<AppInfo>(), new Repository<IdentityUserApp>());
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated) return true;

            var userId = GetUserId(httpContext);
            var appId = GetAppId(httpContext);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(appId)) return false;

            try
            {
                return _appService.IsRegisteredForApp(Guid.Parse(userId), Guid.Parse(appId));
            }
            catch (FormatException)
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(UnauthorizedUrl + "?" + HttpContext.Current.Request.QueryString);
            }
        }

        private static string GetAppId(HttpContextBase httpContext)
        {
            return httpContext.Request["appid"];
        }

        private static string GetUserId(HttpContextBase httpContext)
        {
            var claims = ((System.Security.Claims.ClaimsIdentity) httpContext.User.Identity).Claims;
            var userId = claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return userId;
        }
    }
}