using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Files.Storage
{

    public class LocalFileConfiguration : ILocalFileConfiguration
    {
        public string Path { get; set; }

        public static LocalFileConfiguration FromAppConfig()
        {
            return new LocalFileConfiguration()
            {
                Path = ConfigurationManager.AppSettings["Dragon.Files.Local.Path"]
            };
        }
    }
}
