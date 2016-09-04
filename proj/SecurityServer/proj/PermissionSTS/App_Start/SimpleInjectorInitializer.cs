using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.Identity.Models;
using Dragon.SecurityServer.Identity.Stores;
using Dragon.SecurityServer.PermissionSTS.Models;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;

namespace Dragon.SecurityServer.PermissionSTS
{
    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
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
 
            container.RegisterSingleton(app);
 
            container.RegisterConditional(typeof(IRepository<>), typeof(Repository<>), c => !c.Handled);
 
            container.RegisterPerWebRequest<IDragonUserStore<AppMember>>(() =>
                new Dragon.SecurityServer.ChainedIdentity.Stores.UserStore<AppMember>(new List<IDragonUserStore<AppMember>>{
                    new UserStore<AppMember>(new Repository<AppMember>(), new Repository<IdentityUserClaim>(), new Repository<IdentityUserLogin>(), null, null)
                }));
 
            container.RegisterPerWebRequest(() => ApplicationUserManager.Create(container));

            container.RegisterPerWebRequest(() => container.GetOwinContext().Authentication);

            container.RegisterMvcControllers(
                    Assembly.GetExecutingAssembly());
            
            return container;
        }

        public static IOwinContext GetOwinContext(this Container container)
        {
            return container.IsVerifying()
                ? new OwinContext(new Dictionary<string, object>())
                : HttpContext.Current.GetOwinContext();
        }
    }
}