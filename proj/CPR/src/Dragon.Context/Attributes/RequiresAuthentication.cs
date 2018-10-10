

using System;
using System.Diagnostics;
using System.Web.Mvc;
using Dragon.Common.Extensions;
using Dragon.Context.Configuration;
using StructureMap;
using Dragon.Context.Util;

namespace Dragon.Context.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
       AllowMultiple = false)]
    public class RequiresAuthentication : AuthorizeAttribute
    {
        private const string CONFIG_LOGINURL = "Dragon.Context.LoginUrl";

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (UserID.Equals(Guid.Empty))
            {
                /* not authenticated */
                return false;
            }
            else
            {
                return true;
            }

        }

        protected Guid UserID
        {
            get { return ObjectFactory.GetInstance<DragonContext>().CurrentUserID; }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            Debug.WriteLine(string.Format("Unauthorized request (UserID: '{0}'). Redirecting to login page.", UserID));
            
            var url = LoginUrl;
            var ctx = filterContext.RequestContext.HttpContext;
            var go = ctx.Request.Path;
            go = ctx.Server.UrlEncode(go);
            filterContext.Result = new RedirectResult(url + "?go=" + go);
        }

        public static string LoginUrl
        {
            get
            {
                var url = ObjectFactory.GetInstance<IConfiguration>().EnsureString(CONFIG_LOGINURL);
                return url;
            }
        }
    }
}
