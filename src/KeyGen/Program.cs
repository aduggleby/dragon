using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Common.Util;
using Dragon.Core.Configuration;
using Dragon.Interfaces;
using StructureMap;

namespace KeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectFactory.Container.Inject<IConfiguration>(new ConfigurationManagerConfiguration());

            Console.WriteLine("Dragon Context - Key Generator");

            Console.WriteLine();
            Console.WriteLine("Key: " + CryptUtil.GenerateKey());
            Console.WriteLine("IV:  " + CryptUtil.GenerateIV());
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }
    }
}
