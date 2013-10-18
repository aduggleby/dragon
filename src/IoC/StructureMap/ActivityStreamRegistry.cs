using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.ActivityCenter;
using Dragon.CPR;
using Dragon.CPR.Attributes;
using Dragon.CPR.Impl.Projections;
using Dragon.CPR.Interfaces;
using Dragon.Interfaces;
using Dragon.Interfaces.ActivityCenter;
using StructureMap.Graph;

namespace Dragon.IoC.StructureMap
{
    public class ActivityStreamRegistry : RegistryBase
    {
        public ActivityStreamRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                AssemblyConfig.Assemblies.ForEach(x.Assembly);
                // Implemented and registered from the calling project...
                x.AddAllTypesOf(typeof(IActivityMultiplexer));
                x.AddAllTypesOf(typeof(IActivityService));
                x.AddAllTypesOf(typeof(IEmailProfileService));
                x.AddAllTypesOf(typeof(INotificationDispatcher));
            });

            For<IActivityDispatcher>().Singleton().Use<ActivityDispatcher>();

        }
    }
}
