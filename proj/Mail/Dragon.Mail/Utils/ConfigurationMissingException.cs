using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Utils
{
    public class ConfigurationMissingException : Exception
    {
        private string m_key;
        public ConfigurationMissingException(string key)
        {
            m_key = key;
        }

        public override string Message
        {
            get { return string.Format("Configuration for key '{0}' is missing.", m_key); }
        }
    }
}
