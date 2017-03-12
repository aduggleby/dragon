using System.Web.Mvc;
using System.Web.Routing;

namespace Dragon.SecurityServer.PermissionSTS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*url}", new { url = @"FederationMetadata/2007-06/FederationMetadata.xml" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
