using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common.Attributes.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaAttribute : Attribute
    {
        public SchemaAttribute(string tableName)
        {
            Name = tableName;
        }

        public string Name { get; private set; }
    }

   
}
