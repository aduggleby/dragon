using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Web.Attributes
{
    public class PersistedAsAttribute : Attribute
    {
        public Type As { get; set; }
        public PersistedAsAttribute(Type t)
        {
            As = t;
        }
    }
}
