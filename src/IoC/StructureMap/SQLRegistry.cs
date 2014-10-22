using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.CPR;
using Dragon.Interfaces;
using Dragon.SQL.Repositories;
using StructureMap.Web;

namespace Dragon.IoC.StructureMap
{
    public class SQLRegistry : RegistryBase
    {
        public SQLRegistry()
        {
            For(typeof(IRepository<>)).Use(typeof(Repository<>));
            For<IRepositorySetup>().HybridHttpOrThreadLocalScoped().Use<RepositorySetup>();
            For<PersistableSetup>().Use<PersistableSetup>();
            //FillAllPropertiesOfType<IPermissionStore>();
            //FillAllPropertiesOfType<IProfileStore>();

            Policies.SetAllProperties(
                p => p.TypeMatches(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IRepository<>)));
        }
    }
}
