using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common.Attributes.Data
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute() { }
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
