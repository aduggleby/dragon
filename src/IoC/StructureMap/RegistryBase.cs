using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.CPR.Interfaces;
using SM = StructureMap;

namespace Dragon.IoC.StructureMap
{
    public abstract class RegistryBase : SM.Configuration.DSL.Registry
    {
        protected RegistryBase()
        {
            
        }

        private bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces().ToArray();

            foreach (var it in interfaceTypes)
                if (it.IsGenericType)
                    if (it.GetGenericTypeDefinition() == genericType) return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == genericType) ||
                   IsAssignableToGenericType(baseType, genericType);
        }

        
    }
}
