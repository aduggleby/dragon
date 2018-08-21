using System.Web;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Services;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.Attributes
{
    /// <summary>
    /// Allows access if:
    /// * the IProviderLimiterSelectorService has no limits for the request
    /// </summary>
    public class ProviderRestrictionAttribute : AuthorizeAttribute
    {
        private readonly IProviderLimiterService _providerLimiterService;
        private const string UnauthorizedUrl = "~/Error/Unauthorized";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProviderRestrictionAttribute()
        {
            _providerLimiterService = DependencyResolver.Current.GetService<IProviderLimiterService>();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            Logger.Trace("Checking if access is allowed...");

            if (!_providerLimiterService.HasProviderRestriction(httpContext))
            {
                Logger.Warn("Unable to determine possible provider restrictions, deny access.");
                return false;
            }

            if (_providerLimiterService.DoesAnUnregisteredProviderExist(httpContext))
            {
                _providerLimiterService.RegisterProviderRestriction(httpContext);
            }

            var provider = _providerLimiterService.GetProviderRestriction(httpContext);
            var isAccessAllowed = string.IsNullOrWhiteSpace(provider);
            Logger.Trace((isAccessAllowed ? "Access is allowed." : "Access is not allowed. ") + " Provider: " + provider);
            return isAccessAllowed;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(UnauthorizedUrl + "?" + HttpContext.Current.Request.QueryString);
            }
        }
    }
}