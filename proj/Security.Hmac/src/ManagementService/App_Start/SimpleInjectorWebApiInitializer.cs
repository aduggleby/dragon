using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Modules;
using Dragon.Security.Hmac.Module.Repositories;
using ManagementService.App_Start;
using ManagementService.Util;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof(SimpleInjectorWebApiInitializer), "Initialize")]

namespace ManagementService.App_Start
{
    public static class SimpleInjectorWebApiInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            // Did you know the container can diagnose your configuration? 
            // Go to: https://simpleinjector.org/diagnostics
            var container = new Container();
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            InitializeContainer(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
       
            container.Verify();
            
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
     
        private static void InitializeContainer(Container container)
        {
            var settings = GetSettings();
            var connectionString = GetConnectionString(settings.ConnectionStringName);
            RegisterDbConnection(container, connectionString);
            RegisterRepositories(container, settings);
        }

        private static void RegisterRepositories(Container container, DragonSecurityHmacSection settings)
        {
            container.RegisterWebApiRequest<IAppRepository>(() =>
            {
                var dapperAppRepository = new DapperAppRepository(container.GetInstance<IDbConnection>(), settings.AppsTableName);
                return dapperAppRepository;
            });
            container.RegisterWebApiRequest<IUserRepository>(() =>
            {
                var dapperUserRepository = new DapperUserRepository(container.GetInstance<IDbConnection>(),
                    settings.UsersTableName);
                return dapperUserRepository;
            });
        }

        private static void RegisterDbConnection(Container container, string connectionString)
        {
            container.RegisterWebApiRequest<IDbConnection>(() =>
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            });
        }

        private static string GetConnectionString(string connectionStringName)
        {
            if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
            {
                throw new HmacInvalidConfigException(string.Format("Connection string named {0} is missing.",
                    connectionStringName));
            }
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            return connectionString;
        }

        private static DragonSecurityHmacSection GetSettings()
        {
            const string hmacSectionName = "dragon/security/hmac";
            var settings = (DragonSecurityHmacSection) ConfigurationManager.GetSection(hmacSectionName);
            if (settings == null)
            {
                throw new HmacInvalidConfigException(string.Format("Section {0} is missing.", hmacSectionName));
            }
            return settings;
        }
    }
}