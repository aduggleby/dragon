using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context.Configuration
{
    public class InMemoryConfiguration : ConfigurationBase
    {
        private Dictionary<string, string> m_values;

        public InMemoryConfiguration()
        {
            m_values = new Dictionary<string, string>();
        }

        protected override string GetValueInternal(string key)
        {
            return Get(key);
        }

        public string Get(string key)
        {
            if (!m_values.ContainsKey(key)) return default(string);
            return m_values[key];
        }

        public void Set(string key, object value)
        {
            if (m_values.ContainsKey(key))
            {
                throw new Exception(string.Format("Key '{0}' already set.", key));
            }

            m_values.Add(key, value.ToString());
        }
    }
}
