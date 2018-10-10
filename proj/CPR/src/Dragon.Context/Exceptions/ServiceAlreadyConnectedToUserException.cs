using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context.Exceptions
{
    public class ServiceAlreadyConnectedToUserException : Exception
    {
        public string Service { get; set; }
    }
}
