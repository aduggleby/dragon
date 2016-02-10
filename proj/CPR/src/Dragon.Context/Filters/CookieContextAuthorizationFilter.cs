using System.Web.Mvc;

namespace Dragon.Context.Filters
{
    public class CookieContextAuthorizationFilter : IAuthorizationFilter
    {
        public IContext Ctx { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            Ctx.Load();
        }
    }
}
