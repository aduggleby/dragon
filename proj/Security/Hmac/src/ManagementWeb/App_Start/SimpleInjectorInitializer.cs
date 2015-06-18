using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using ManagementWeb.App_Start;
using ManagementWeb.Areas.Hmac.Models;
using ManagementWeb.Areas.Hmac.Repositories;
using ManagementWeb.Util;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof(SimpleInjectorInitializer), "Initialize")]

namespace ManagementWeb.App_Start
{
    public static class SimpleInjectorInitializer
    {
        private const string HmacManagementApiUrlSettingName = "MediaServer.Management.HmacManagementApiUrl";

        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            // Did you know the container can diagnose your configuration? 
            // Go to: https://simpleinjector.org/diagnostics
            var container = new Container();
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();
            
            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
     
        private static void InitializeContainer(Container container)
        {
            var hmacManagementApiUrl = ConfigurationManager.AppSettings[HmacManagementApiUrlSettingName];
            container.Register<IGenericRepository<AppModel, int?>, AppRepository>();
            container.RegisterInitializer<IGenericRepository<AppModel, int?>>(x => x.ServiceUrl = hmacManagementApiUrl + "/App");
            container.Register<IGenericRepository<UserModel, long?>, UserRepository>();
            container.RegisterInitializer<IGenericRepository<UserModel, long?>>(x => x.ServiceUrl = hmacManagementApiUrl + "/User");
        }
    }
}