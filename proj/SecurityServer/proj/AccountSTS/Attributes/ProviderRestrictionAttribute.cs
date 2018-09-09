using System;
using System.Web;
using System.Web.Mvc;

namespace Dragon.SecurityServer.AccountSTS.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Allows access if:
    /// * the IProviderLimiterSelectorService has no limits for the request
    /// </summary>
    public class ProviderRestrictionAttribute : ProviderAttributesBase
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Authorize(httpContext, false);
        }

        /// <summary>
        /// Check provider restrictions even if an AllowAnonymousAttribute is set.
        /// </summary>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!ProviderLimiterService.IsEnabled())
            {
                base.OnAuthorization(filterContext);
                return;
            }

            var isAllowAnonymousAttributeDefined = IsAttributeDefined<AllowAnonymousAttribute>(filterContext);
            var isProviderRestrictionAttributeDefined = IsAttributeDefined<ProviderRestrictionAttribute>(filterContext);
            if (!(isAllowAnonymousAttributeDefined && isProviderRestrictionAttributeDefined))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // See AuthorizeAttribute::OnAuthorization
                if (Authorize(filterContext.HttpContext, isAllowAnonymousAttributeDefined))
                {
                    var cache = filterContext.HttpContext.Response.Cache;
                    cache.SetProxyMaxAge(new TimeSpan(0L));
                    cache.AddValidationCallback(CacheValidateHandler, null);
                }
                else
                {
                    HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        // See AuthorizeAttribute::CacheValidationHandler
        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        private static bool IsAttributeDefined<T>(AuthorizationContext filterContext) where T : class
        {
            return filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                   filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
        }
    }
}