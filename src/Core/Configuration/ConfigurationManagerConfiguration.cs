using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Core.Configuration
{
   public class ConfigurationManagerConfiguration: ConfigurationBase
   {
       protected override string GetValueInternal(string key)
       {
           return ConfigurationManager.AppSettings[key];
       }
   }
}
