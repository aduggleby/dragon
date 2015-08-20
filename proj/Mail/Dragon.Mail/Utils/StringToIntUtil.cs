using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Dragon.Mail.Utils
{
    public static class StringToIntUtil
    {
        private static ILog s_log = LogManager.GetCurrentClassLogger();

        public static int Interpret(string s, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;

            int i;
            if (!Int32.TryParse(s, out i))
            {
                s_log.Warn("Configuration value '" + s +
                           "' expected integer, but could not interpret. Using default value: " + defaultValue);
                return defaultValue;
            }

            return i;
        }
    }
}
