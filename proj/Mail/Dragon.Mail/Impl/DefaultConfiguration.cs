using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;

namespace Dragon.Mail.Impl
{
    public class DefaultConfiguration : IConfiguration
    {
        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
