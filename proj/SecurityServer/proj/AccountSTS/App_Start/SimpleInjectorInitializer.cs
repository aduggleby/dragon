using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Modules;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.AccountSTS.Services.CheckPasswortServices;
using Dragon.SecurityServer.AccountSTS.WebRequestHandler;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using StackExchange.Redis;
using StackRedis.AspNet.Identity;
using IdentityUser = Dragon.SecurityServer.Identity.Models.IdentityUser;

namespace Dragon.SecurityServer.AccountSTS
{
    internal class ImportPropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type type, PropertyInfo prop)
        {
            return prop.GetCustomAttributes(typeof(ImportAttribute)).Any();
        }
    }

    public static class SimpleInjectorInitializer
    {
        private const string RedisConnectionStringName = "Redis";

        public static Container Initialize(IAppBuilder app)
        {
            var container = GetInitializeContainer(app);

            var registration = container.GetRegistration(typeof(IAuthenticationManager)).Registration;
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposed by MVC");
            registration = container.GetRegistration(typeof(IDragonUserStore<AppMember>)).Registration;
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposed by MVC");
            var producer = container.GetRegistration(typeof(UserStore<IdentityUser>));
            if (producer != null)
            {
                registration = producer.Registration;
                registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposed by MVC");
            }
            registration = container.GetRegistration(typeof(ApplicationSignInManager)).Registration;
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposed by MVC");
             
            container.Verify();
 
            DependencyResolver.SetResolver(
                new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
       
            return container;
        }
 
        public static Container GetInitializeContainer(
                  IAppBuilder app)
        {
            var container = new Container();
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior(); // Enable ImportAttribute for property injection
 
            container.RegisterSingleton(app);

            container.RegisterSingleton(GetHmacSettings);

            container.RegisterSingleton<Func<IDbConnection, HmacHttpService>>(connection =>
            {
                var hmacSettings = GetHmacSettings();
                return new HmacHttpService(hmacSettings.ServiceId, hmacSettings.Paths, hmacSettings.SignatureParameterKey)
                {
                    HmacService = new HmacSha256Service(),
                    UserRepository = new DapperUserRepository(connection, hmacSettings.UsersTableName),
                    AppRepository = new DapperAppRepository(connection, hmacSettings.AppsTableName),
                };
            });
 
            container.RegisterConditional(typeof(IRepository<>), typeof(Repository<>), c => !c.Handled);

            container.Register<ICheckPasswordService<AppMember>, LegacyCheckPasswordService<AppMember>>(Lifestyle.Transient);
 
            container.RegisterPerWebRequest<IDragonUserStore<AppMember>>(() =>
            {
                var dragonUserStores = new List<IDragonUserStore<AppMember>>
                {
                    new UserStore<AppMember>(new Repository<AppMember>(), null, new Repository<IdentityUserLogin>(),
                        new Repository<IdentityService>())
                };
                if (WebConfigurationManager.ConnectionStrings[RedisConnectionStringName] != null)
                {
                    var connectionMultiplexer = ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings[RedisConnectionStringName].ConnectionString);
                    connectionMultiplexer.PreserveAsyncOrder = false;
                    dragonUserStores.Insert(0, new Identity.Redis.UserStore<AppMember>(new RedisUserStore<Identity.Redis.IdentityUser>(connectionMultiplexer), connectionMultiplexer));
                }
                var userStore = new ChainedIdentity.Stores.UserStore<AppMember>(dragonUserStores);
                Startup.OpenIdMigrationWebrequestHandler = new OpenIdMigrationWebRequestHandler(userStore);
                return userStore;
            });

            container.RegisterPerWebRequest(() => ApplicationUserManager.Create(container, Startup.DataProtectionProvider));

            container.RegisterPerWebRequest(() => container.GetOwinContext().Authentication);

            container.RegisterMvcControllers(
                    Assembly.GetExecutingAssembly());
            
            return container;
        }

        private static DragonSecurityHmacSection GetHmacSettings()
        {
            var settings = (DragonSecurityHmacSection) ConfigurationManager.GetSection(Consts.HmacSectionName);
            if (settings == null)
            {
                throw new HmacInvalidConfigException($"Section {Consts.HmacSectionName} is missing.");
            }
            return settings;
        }

        public static IOwinContext GetOwinContext(this Container container)
        {
            return container.IsVerifying()
                ? new OwinContext(new Dictionary<string, object>())
                : HttpContext.Current.GetOwinContext();
        }
    }
}
