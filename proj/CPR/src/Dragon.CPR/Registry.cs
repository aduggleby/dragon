//using System;
//using System.Diagnostics;
//using System.Linq;
//using Dragon.CPR.Impl;
//using Dragon.CPR.Impl.Projections;
//using Dragon.Data.Interfaces;
//using Dragon.CPR.Sql;
//using Dragon.Context;
//using Dragon.Interfaces;
//using StructureMap;
//using StructureMap.Graph;

//namespace Dragon.CPR
//{
//    public class Registry : StructureMap.Configuration.DSL.Registry
//    {
//        public Registry()
//        {
//            For<ISqlConnectionFactory>().Singleton().Use<SqlConnectionFactory>();
//            For<IReadModelRepository>().HybridHttpOrThreadLocalScoped().Use<DapperRepository>();
//            Forward<IReadModelRepository, ISetupRepository>();
//            Forward<IReadModelRepository, IDropRepository>();
//            Forward<IReadModelRepository, IReadRepository>();
//            Forward<IReadModelRepository, IWriteRepository>();
//            Forward<IWriteRepository, ICommandRepository>();

//            Scan(x =>
//            {
//                x.TheCallingAssembly();
//                Config.Assemblies.ForEach(a => x.Assembly(a));
//                x.AddAllTypesOf(typeof(IInterceptor<>));
//            });

//            /*var types = Config.Assemblies
//                              .SelectMany(a => a.GetTypes());*/

//            //foreach (var type in types)
//            //{
//            //    foreach (var iface in type.GetInterfaces())
//            //    if (iface.IsGenericType)
//            //    if (typeof(IInterceptor<>).IsAssignableFrom(iface.GetGenericTypeDefinition()))
//            //    {
//            //        var interceptsClass = iface.GetGenericArguments().First();

//            //        var alsoInterceptsClasses = types.Where(t => interceptsClass.IsAssignableFrom(t)).ToList();

//            //        alsoInterceptsClasses.ForEach(t =>
//            //            {
//            //                var ifaceForHandler = typeof(IInterceptor<>).MakeGenericType(t);

//            //                For(ifaceForHandler).Use(type);
//            //            });
//            //    }

//            //}


//            var commands = Config.Assemblies
//                                .SelectMany(a => a.GetTypes())
//                               .Where(x => !x.IsAbstract)
//                               .Where(x => typeof(CommandBase).IsAssignableFrom(x));

//            foreach (var command in commands)
//            {
//                var handlerInterface = typeof(IHandler<>).MakeGenericType(command);
//                var projectionInterface = typeof(IProjection<>).MakeGenericType(command);

//                Scan(x =>
//                {
//                    x.TheCallingAssembly();
//                    Config.Assemblies.ForEach(a => x.Assembly(a));
//                    x.AddAllTypesOf(handlerInterface);
//                    x.AddAllTypesOf(projectionInterface);
//                });

//                //var projectionInterface = typeof(IProjection<>).MakeGenericType(cmdType);
//                var dispatcher = typeof(CommandDispatcher<>).MakeGenericType(command);
//                SetAllProperties(a => a.TypeMatches(p => p.Equals(dispatcher)));

//                // 
//                var autoProject = command.GetCustomAttributes(typeof(AutoProjectToAttribute), true).FirstOrDefault() as AutoProjectToAttribute;
//                if (autoProject != null)
//                {
//                    var commandProjectionImpl = typeof(CommandProjection<,>).MakeGenericType(command, autoProject.Type);
//                    For(projectionInterface).Use(commandProjectionImpl);
//                }

//                //    var genericBaseType = GetGenericType(automapType, typeof(AutoMapCommandBase<>));
//                //    var genericAggregateType = genericBaseType.GetGenericArguments()[0];

//                //    var isAbstract = genericAggregateType.DeclaringType != null && genericAggregateType.DeclaringType.IsAbstract;

//                //    if (!isAbstract)
//                //    {
//                //        var commandHandler = typeof(AutoMappingCommandHandler<,>).MakeGenericType(automapType, genericAggregateType);

//                //        For(subscriberForAutomap).Use(commandHandler);
//                //    }
//            }


//            SetAllProperties(a => a.TypeMatches(p => p.IsArray && typeof(IInterceptor<>).IsAssignableFrom(p.GetElementType().GetGenericTypeDefinition())));


     

//        }

//        private bool IsAssignableToGenericType(Type givenType, Type genericType)
//        {
//            var interfaceTypes = givenType.GetInterfaces();

//            foreach (var it in interfaceTypes)
//                if (it.IsGenericType)
//                    if (it.GetGenericTypeDefinition() == genericType) return true;

//            Type baseType = givenType.BaseType;
//            if (baseType == null) return false;

//            return (baseType.IsGenericType &&
//                baseType.GetGenericTypeDefinition() == genericType) ||
//                IsAssignableToGenericType(baseType, genericType);
//        }

//        public class InterceptorConvention : ITypeScanner
//        {
//            public void Process(Type type, PluginGraph graph)
//            {
//                if (typeof(IInterceptor<>).IsAssignableFrom(type))
//                {
//                    var interceptedType = type.GetGenericArguments().First();

//                    var allSubclasses = CPR.Config.Assemblies.SelectMany(x => x.GetTypes()).Where(t => t.IsSubclassOf(interceptedType));

//                    foreach (var subclass in allSubclasses)
//                    {
//                        Debug.WriteLine("Registering subclass " + subclass.ToString() + " to " + type.ToString());
//                        var c = subclass;
//                        graph.Configure(x => x.For(c).Use(type));
//                    }
//                }
//            }
//        }
//    }
//}
