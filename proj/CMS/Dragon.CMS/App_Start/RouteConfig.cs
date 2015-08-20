using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dragon.CMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Modules",
                url: "_modules/{action}",
                defaults: new { controller = "Module" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Page", action = "Render" }
            );
        }
    }
}
