using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Dragon.Configuration
{
   public class ConfigurationManagerConfiguration: ConfigurationBase
   {
       protected override string GetValueInternal(string key)
       {
           return ConfigurationManager.AppSettings[key];
       }
   }
}
