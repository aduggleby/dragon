using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.CPR;
using Dragon.CPR.Attributes;
using Dragon.CPR.Impl.Projections;
using Dragon.CPR.Interfaces;
using StructureMap.Graph;

namespace Dragon.IoC.StructureMap
{
    public class CPRRegistry : RegistryBase
    {
        public CPRRegistry()
        {

            Scan(x =>
            {
                x.TheCallingAssembly();
                AssemblyConfig.Assemblies.ForEach(x.Assembly);
                x.AddAllTypesOf(typeof(IInterceptor<>));
            });

            var commands = AssemblyConfig.Assemblies
                              .SelectMany(a => a.GetTypes())
                             .Where(x => !x.IsAbstract)
                             .Where(x => typeof(CommandBase).IsAssignableFrom(x));

            foreach (var command in commands)
            {
                var handlerInterface = typeof(IHandler<>).MakeGenericType(command);
                var projectionInterface = typeof(IProjection<>).MakeGenericType(command);

                Scan(x =>
                {
                    x.TheCallingAssembly();
                    AssemblyConfig.Assemblies.ForEach(x.Assembly);
                    x.AddAllTypesOf(handlerInterface);
                    x.AddAllTypesOf(projectionInterface);
                });


                //var projectionInterface = typeof(IProjection<>).MakeGenericType(cmdType);
                var dispatcher = typeof(CommandDispatcher<>).MakeGenericType(command);
                SetAllProperties(a => a.TypeMatches(p => p.Equals(dispatcher)));

                // 
                var autoProject =
                    command.GetCustomAttributes(typeof(AutoProjectToAttribute), true).FirstOrDefault() as
                        AutoProjectToAttribute;
                if (autoProject != null)
                {
                    var commandProjectionImpl = typeof(CommandProjection<,>).MakeGenericType(command, autoProject.Type);
                    For(projectionInterface).Use(commandProjectionImpl);
                }
            }

            //SetAllProperties(a =>
            //{
            //    Debug.WriteLine("A:" + a);
            //    a.TypeMatches(
            //        p =>
            //        {
            //            Debug.WriteLine("P:" + p);
            //            return p.IsArray &&
            //                   typeof(IInterceptor<>).IsAssignableFrom(p.GetElementType().GetGenericTypeDefinition());
            //        });
            //});
        }
    }

    public class InterceptorConvention : ITypeScanner
    {
        public void Process(Type type, PluginGraph graph)
        {
            if (typeof(IInterceptor<>).IsAssignableFrom(type))
            {
                var interceptedType = type.GetGenericArguments().First();

                var allSubclasses =
                    AssemblyConfig.Assemblies.SelectMany(x => x.GetTypes()).Where(t => t.IsSubclassOf(interceptedType));

                foreach (var subclass in allSubclasses)
                {
                    Debug.WriteLine("Registering subclass " + subclass.ToString() + " to " + type.ToString());
                    var c = subclass;
                    graph.Configure(x => x.For(c).Use(type));
                }
            }
        }
    }
}
