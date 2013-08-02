using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common.Attributes.Data
{

    [AttributeUsage(AttributeTargets.Property)]
    public class NoColumnAttribute : Attribute
    {
        public NoColumnAttribute()
        {
           
        }
    }
}
