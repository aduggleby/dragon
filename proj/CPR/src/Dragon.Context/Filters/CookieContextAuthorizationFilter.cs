using System;
using System.Web.Mvc.Filters;

namespace Dragon.Context.Filters
{
    public class CookieContextAuthorizationFilter : IAuthenticationFilter
    {
        /// <summary>
        /// The same context as the controller is needed, therefore always retrieve an up to date context.
        /// </summary>
        public Func<IContext> CtxRetriever { get; set; }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            CtxRetriever().Load();
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            // nothing to be done
        }
    }
}
