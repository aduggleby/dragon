using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Configuration
{
    public abstract class ConfigurationBase : IConfiguration
    {
        public ConfigurationBase()
        {
          
        }

        protected abstract string GetValueInternal(string key);

        public T GetValue<T>(string configKey, T defaultValue)
        {
            var value = GetValueInternal(configKey);

            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public T EnsureValue<T>(string configKey)
        {
            var value = GetValueInternal(configKey);

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception(string.Format("Expected configuration value '{0}' not found.", configKey));

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
