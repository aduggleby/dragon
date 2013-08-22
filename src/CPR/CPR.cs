using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dragon.CPR
{
    public static class Config
    {
        static Config()
        {
            Assemblies = new List<Assembly>();
        }

        public static List<Assembly> Assemblies { get; set; }
    }
}
