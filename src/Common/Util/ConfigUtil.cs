using System;
using System.Configuration;

namespace Dragon.Common.Util
{
    public static class ConfigUtil
    {
        public static T GetValueFromWebConfig<T>(string configKey, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[configKey];

            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return (T)Convert.ChangeType(value, typeof(T)); 
        }

        public static string GetAndEnsureValueFromWebConfig(string configKey)
        {
            var value = ConfigurationManager.AppSettings[configKey];

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception(string.Format("Expected configuration value '{0}' not found.", configKey));

            return value;
        }

        public static bool IsTrue(string configKey)
        {
            return ConfigUtil.GetAndEnsureValueFromWebConfig(configKey)
                                 .Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
            
        }

    

    }
}
