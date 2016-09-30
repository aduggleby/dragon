using System;
using System.IdentityModel.Claims;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Helpers;
using Dragon.SecurityServer.AccountSTS.Services;
using Dragon.SecurityServer.Common;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.Attributes
{
    /// <summary>
    /// Allows access if:
    /// * the request contains the userid and appid properties
    /// * the user is not registered to an app in the same group as the requested app already (<see cref="Identity.Models.IdentityUserApp" />, <see cref="Models.AppInfo" />)
    /// Also handles app/service registration of logged in users.
    /// </summary>
    public class AuthorizeForRegisteredAppsAttribute : AuthorizeAttribute
    {
        private const string UnauthorizedUrl = "~/Error/Unauthorized";

        private readonly IAppService _appService;
        private readonly IUserService _userService;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AuthorizeForRegisteredAppsAttribute()
        {
            _userService = DependencyResolver.Current.GetService<IUserService>();
            _appService = DependencyResolver.Current.GetService<IAppService>();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            Logger.Trace("Checking if access is allowed...");

            if (!httpContext.User.Identity.IsAuthenticated) return true;

            var userId = GetUserId(httpContext);
            var appId = GetAppId(httpContext);
            var serviceId = RequestHelper.GetCurrentServiceId();

            Logger.Trace($"App: {appId} / User: {userId} / Service: {serviceId}");

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(appId)) return false;

            try
            {
                // Login/signup to apps in different groups is allowed, so do not use _appService.IsRegisteredForApp
                var isAccessAllowed = _appService.IsAllowedToAccessApp(Guid.Parse(userId), Guid.Parse(appId));
                Logger.Trace("Is access allowed? " + isAccessAllowed);
                if (isAccessAllowed)
                {
                    RegisterUserForAppAndService(userId, appId, serviceId);
                }
                return isAccessAllowed;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Ensure that already logged in users can not arbitrarily access apps.
        /// Since ApplicationSignInManager::PostSignIn is not called in this case, the user is registered for new apps/services in this method.
        /// </summary>
        private void RegisterUserForAppAndService(string userId, string appId, string serviceId)
        {
            Logger.Trace("Registering app and service to the user...");
            var user = AsyncRunner.Run(_userService.GetUser(userId));
            AsyncRunner.Run(_userService.AddCurrentServiceIdToUserIfNotAlreadyAdded(user, serviceId));
            AsyncRunner.Run(_userService.AddCurrentAppIdToUserIfNotAlreadyAdded(user, appId));
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