using System.Web;
using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Services;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.Attributes
{
    public abstract class ProviderAttributesBase : AuthorizeAttribute
    {
        protected readonly IProviderLimiterService ProviderLimiterService;
        private const string UnauthorizedUrl = "~/Error/Unauthorized";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected ProviderAttributesBase()
        {
            ProviderLimiterService = DependencyResolver.Current.GetService<IProviderLimiterService>();
        }

        protected bool Authorize(HttpContextBase httpContext, bool isAllowAnonymousAttributeDefined)
        {
            if (!ProviderLimiterService.IsEnabled())
            {
                return AuthorizeCore(httpContext);
            }

            Logger.Trace("Checking if access is allowed...");

            if (!ProviderLimiterService.HasProviderRestriction(httpContext))
            {
                Logger.Warn("Unable to determine possible provider restrictions, deny access.");
                return false;
            }

            if (ProviderLimiterService.DoesAnUnregisteredProviderExist(httpContext))
            {
                ProviderLimiterService.RegisterProviderRestriction(httpContext);
            }

            var provider = ProviderLimiterService.GetProviderRestriction(httpContext);
            var isAccessAllowed = string.IsNullOrWhiteSpace(provider);
            Logger.Trace((isAccessAllowed ? "Access is allowed." : "Access is not allowed. ") + " Provider: " + provider);
            return isAccessAllowed && (isAllowAnonymousAttributeDefined || AuthorizeCore(httpContext));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Result = new RedirectResult(UnauthorizedUrl + "?" + HttpContext.Current.Request.QueryString);
        }
    }
}