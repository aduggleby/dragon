using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Demo.DependencyResolution;
using Dragon.Core.Sql;
using Dragon.Notification;

namespace Demo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebNotificationDispatcher.Init();
            WebNotificationDispatcher.NotificationHub.Dispatcher = new WebNotificationDispatcher(
                new StringTemplateTemplateService(),
                new FileSystemLocalizedDataSource(HttpContext.Current.Server.MapPath("~/Resources/templates"), "txt"),
                new SqlNotificationStore(StandardSqlStore.ConnectionString)
                );

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());
        }
    }
}