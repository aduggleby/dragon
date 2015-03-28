using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Dragon.Web.Demo.CPR;
using Dragon.Web.Interfaces;
using Dragon.Web.Utils;
using Microsoft.Owin;
using Owin;
using SimpleInjector;
using SimpleInjector.Extensions;
using SimpleInjector.Integration.Web.Mvc;

[assembly: OwinStartup(typeof(Dragon.Web.Demo.Startup))]

namespace Dragon.Web.Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DependencyConfig.Register();
        }
    }

    public class DependencyConfig
    {
        public static void Register()
        {
            // Create the container as usual.
            var container = new Container();
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            // Subs
            Dragon.Web.DependencyConfig.Register(container);

            // This is an extension method from the integration package.
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            // This is an extension method from the integration package as well.
            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();

            var x = container.GetAllInstances<ICommandSave<DemoDeleteCommand>>();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }

  
}