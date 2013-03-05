using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Common.Extensions
{
    public static class IConfigurationExtensions
    {
        private static string[] TRUE_VALUES = new string[] {"true", "t", "1", "yes", "y", "on"};
        
        public static T GetValue<T>(this IConfiguration cfg, string configKey)
        {
            return cfg.GetValue<T>(configKey, default(T));
        }

        public static string EnsureString(this IConfiguration cfg, string configKey)
        {
            return cfg.EnsureValue<string>(configKey);
        }

        public static string GetString(this IConfiguration cfg, string configKey)
        {
            return GetValue<string>(cfg, configKey);
        }

        public static int GetInt(this IConfiguration cfg, string configKey)
        {
            return GetValue<int>(cfg, configKey);
        }

        public static string GetString(this IConfiguration cfg, string configKey, string defaultValue)
        {
            return cfg.GetValue<string>(configKey, defaultValue);
        }

        public static int GetInt(this IConfiguration cfg, string configKey, int defaultValue)
        {
            return cfg.GetValue<int>(configKey, defaultValue);
        }
        
        public static bool IsTrue(this IConfiguration cfg, string configKey)
        {
            var value = GetString(cfg, configKey);
            return TRUE_VALUES.Select(x=>x.ToLower()).Contains(value.ToLower());
        }

        public static bool IsFalse(this IConfiguration cfg, string configKey)
        {
            return !IsTrue(cfg, configKey);
        }
       

    }
}
