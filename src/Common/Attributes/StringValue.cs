using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common.Attributes
{
    public class StringValue : System.Attribute
    {
        private string m_value;

        public StringValue(string value)
        {
            m_value = value;
        }

        public string Value
        {
            get { return m_value; }
        }
    } 
}
