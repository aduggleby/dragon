using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Attributes
{
    public class AutoProjectToAttribute : Attribute
    {
        public Type Type { get; set; }

        public AutoProjectToAttribute(Type t)
        {
            Type = t;
        }
    }
}
