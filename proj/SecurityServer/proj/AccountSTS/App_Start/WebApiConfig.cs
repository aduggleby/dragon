using System.Web.Http;

namespace Dragon.SecurityServer.AccountSTS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "Api/{action}/{id}",
                defaults: new { controller = "AccountApi", id = RouteParameter.Optional }
            );
        }
    }
}
