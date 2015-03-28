using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Dragon.Web.Defaults;
using Dragon.Web.Interfaces;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace Dragon.Web
{
    public class DependencyConfig
    {
        public static void Register(Container container)
        {
            container.RegisterOpenGeneric(typeof(IRepository<>), typeof(Repository<>));

            // Single services
            container.Register<IContext, DefaultContext>();

            AddInterfaceForEachDerivedType(container, typeof(TableBase), typeof(ITableBeforeSave<>));
            AddInterfaceForEachDerivedType(container, typeof(TableBase), typeof(ITableAfterSave<>));

            AddInterfaceForEachDerivedType(container, typeof(CommandBase), typeof(ICommandBeforeProcess<>));
            AddInterfaceForEachDerivedType(container, typeof(CommandBase), typeof(ICommandBeforeSave<>));
            AddInterfaceForEachDerivedType(container, typeof(CommandBase), typeof(ICommandAfterSave<>));

            // default persister
            container.RegisterOpenGeneric(typeof(ICommandSave<>), typeof(DefaultSaver<>));

            // override persisters
            container.Options.AllowOverridingRegistrations = true;

            container.RegisterManyForOpenGeneric(typeof(ICommandSave<>), AppDomain.CurrentDomain.GetAssemblies());

        }

        private static void AddInterfaceForEachDerivedType(
            Container container,
            Type baseType,
            Type interfaceType)
        {
            var allTypesDerivedFromBase =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(baseType.IsAssignableFrom)
                    .Where(x => !x.IsAbstract)
                    .Distinct()
                    .ToArray();

            var genericInterface = interfaceType.GetGenericTypeDefinition();

            foreach (var concrete in allTypesDerivedFromBase)
            {
                var concreteInterface = genericInterface.MakeGenericType(concrete);

                var concreteImplementations = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.GetInterfaces().Any())
                    .Where(concreteInterface.IsAssignableFrom)
                    .Distinct()
                    .ToList();

                container.RegisterAll(concreteInterface, concreteImplementations);

            }
        }
    }
}
