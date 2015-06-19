Dragon.Security.Hmac.ManagementWeb
==================================

This package adds an ASP.NET MVC Area into existing ASP.NET projects. The area allows managing users and apps of the Hmac module.


Requirements
------------

* A running Dragon.Security.Hmac.ManagementService service
* An existing ASP.NET MVC 5 project


Setup
-----

* Add the NuGet package

* Configure the ManagementService location in the Web.config file (appSettings), e.g.:

        <add key="MediaServer.Management.HmacManagementApiUrl" value="http://localhost:14502/api" />

* Setup dependency injection

  - Controllers: inject repositories
  - Repositories: setup service URLs

  e.g. SimpleInject (using the ImportPropertySelectionBehavior):

        private const string HmacManagementApiUrlSettingName = "MediaServer.Management.HmacManagementApiUrl";

        ...

        private static void InitializeContainer(Container container)
        {
            var hmacManagementApiUrl = ConfigurationManager.AppSettings[HmacManagementApiUrlSettingName];
            container.Register<IGenericRepository<AppModel, int?>, AppRepository>();
            container.RegisterInitializer<IGenericRepository<AppModel, int?>>(x => x.ServiceUrl = hmacManagementApiUrl + "/App");
            container.Register<IGenericRepository<UserModel, long?>, UserRepository>();
            container.RegisterInitializer<IGenericRepository<UserModel, long?>>(x => x.ServiceUrl = hmacManagementApiUrl + "/User");
        }
