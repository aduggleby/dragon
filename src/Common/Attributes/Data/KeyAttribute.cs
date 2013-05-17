using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common.Attributes.Data
{
    // do not want to depend on data annotations that is not in client profile
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }
}
