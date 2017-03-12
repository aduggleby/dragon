using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Data.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class NoColumnAttribute : Attribute
    {
        public NoColumnAttribute()
        {
           
        }
    }
}
