using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Configuration
{
    public static class AssemblyConfig
    {
        private static List<Assembly> m_assemblies = new List<Assembly>();

        public static List<Assembly> Assemblies
        {
            get { return m_assemblies; }
        }
    }
}
