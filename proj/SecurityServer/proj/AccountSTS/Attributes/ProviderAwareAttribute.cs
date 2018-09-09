using System.Web.Mvc;
using Dragon.SecurityServer.AccountSTS.Services;

namespace Dragon.SecurityServer.AccountSTS.Attributes
{
    /// <summary>
    /// Attach to actions/controllers that should not be restricted themselves, but register and set provider restrictions
    /// for actions/controllers restricted by the ProviderRestrictionAttribute.
    /// </summary>
    public class ProviderAwareAttribute : ActionFilterAttribute
    {
        private readonly IProviderLimiterService _providerLimiterService;

        public ProviderAwareAttribute()
        {
            _providerLimiterService = DependencyResolver.Current.GetService<IProviderLimiterService>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!_providerLimiterService.IsEnabled())
            {
                return;
            }

            var httpContext = filterContext.HttpContext;
            if (_providerLimiterService.DoesAnUnregisteredProviderExist(httpContext))
            {
                _providerLimiterService.RegisterProviderRestriction(httpContext);
            }
        }
    }
}