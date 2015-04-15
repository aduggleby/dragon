using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            container.Register<ICommandPersister, DefaultCommandPersister>();
            container.Register<ICommandSerializer, NewtonsoftCommandSerializer>();
            //container.Register<ICommandSerializer, JilCommandSerializer>();

            AddInterfaceForEachDerivedType(container, typeof(TableBase), typeof(ITableBeforeSave<>));
            AddInterfaceForEachDerivedType(container, typeof(TableBase), typeof(ITableAfterSave<>));

            AddInterfaceForEachDerivedType(container, typeof(CommandBase), typeof(ICommandBeforeProcess<>));
            AddInterfaceForEachDerivedType(container, typeof(CommandBase), typeof(ICommandBeforeSave<>));
            AddInterfaceForEachDerivedType(container, typeof(CommandBase), typeof(ICommandAfterSave<>));

            // default persister
            container.RegisterOpenGeneric(typeof(ICommandSave<>), typeof(DefaultSaver<>));

            // override persisters
            container.Options.AllowOverridingRegistrations = true;

            //container.RegisterOpenGeneric(typeof(InsertCommandFor<>), typeof(SimpleCommandsDefaultSaver<>));
            //container.RegisterManyForOpenGeneric(typeof(ICommandSave<>), AppDomain.CurrentDomain.GetAssemblies());
            //container.RegisterManyForOpenGeneric(typeof(ICommandSave<>).MakeGenericType(typeof(InsertCommandFor<>)), AppDomain.CurrentDomain.GetAssemblies());
            /*
            ReplaceInterfaceForEachDerivedType(container, 
                typeof(SimpleCommand<>), 
                typeof(ICommandSave<>).MakeGenericType(typeof(InsertCommandFor<>))
                );*/

            //RegisterAllPersisters();

        }

        //private static void RegisterAllPersisters()
        //{
        //    var baseType = typeof(CommandBase);

        //    var allCommands =
        //       AppDomain.CurrentDomain.GetAssemblies()
        //           .SelectMany(x => x.GetTypes())
        //           .Where(baseType.IsAssignableFrom)
        //           .Where(x => !x.IsAbstract)
        //           .Distinct()
        //           .ToArray();

        //    foreach (var command in allCommands)
        //    {
        //        if (command.IsGenericType)
        //        {

        //            // command.GetGenericArguments()[0].GetGenericParameterConstraints()
        //            // go throught all of that type and find the icommandsaves for it!
        //            var x = command.GetGenericParameterConstraints();
        //        }
        //        else
        //        {
        //            var saverType = typeof(ICommandSave<>).MakeGenericType(command);

        //            var allCommandSavers =
        //                AppDomain.CurrentDomain.GetAssemblies()
        //                    .SelectMany(x => x.GetTypes())
        //                    .Where(saverType.IsAssignableFrom)
        //                    .Where(x => !x.IsAbstract)
        //                    .Distinct()
        //                    .ToArray();
        //        }
        //    }
        //}

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
                Trace.Write("Interface " + concreteInterface.ToString());

                if (!concrete.IsGenericType)
                {

                    var concreteImplementations = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Where(x => x.GetInterfaces().Any())
                        .Where(concreteInterface.IsAssignableFrom)
                        .Distinct()
                        .ToList();

                    if (concreteImplementations.Any())
                    {
                        Trace.WriteLine(" implemented by:");
                        foreach (var impl in concreteImplementations)
                        {
                            Trace.WriteLine("-> " + impl.ToString());
                        }

                        container.RegisterAll(concreteInterface, concreteImplementations);
                    }
                    else
                    {
                        Trace.WriteLine(" has not implementations.");
                    }

                }
                else
                {
                    Trace.WriteLine("'s generic type param is itself generic and must be registered manually.");
                }
            }
        }

        private static void ReplaceInterfaceForEachDerivedType(
         Container container,
         Type baseType,
         Type interfaceType)
        {
            var xxx = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()).Where(x => x.FullName.Contains("InsertCommandFor"));

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
                Trace.Write("Interface " + concreteInterface.ToString());

                if (!concrete.IsGenericType)
                {

                    var concreteImplementations = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Where(x => x.GetInterfaces().Any())
                        .Where(concreteInterface.IsAssignableFrom)
                        .Distinct()
                        .ToList();

                    if (concreteImplementations.Count() > 1)
                    {
                        throw new Exception(string.Format("Interface {0} has multiple implementations.", concreteInterface.ToString()));
                    }
                    else if (concreteImplementations.Count() == 1)
                    {

                        Trace.WriteLine(" implemented by:");
                        foreach (var impl in concreteImplementations)
                        {
                            Trace.WriteLine("-> " + impl.ToString());
                        }

                        container.Register(concreteInterface, concreteImplementations.First());

                    }
                    else
                    {
                        Trace.WriteLine(" has not implementations.");
                    }

                }
                else
                {
                    Trace.WriteLine("'s generic type param is itself generic and must be registered manually.");
                }
            }
        }
    }
}
