using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Data.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : Attribute
    {
        public LengthAttribute() { }
        public LengthAttribute(string l)
        {
            Length = l;
        }
        public LengthAttribute(int i)
        {
            Length = i.ToString();
        }
        public string Length { get; set; }
    }
}
