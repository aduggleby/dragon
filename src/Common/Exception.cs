using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common
{
    public static class Ex
    {
        public static Exception For(string msg, params string[] args)
        {
            return new Exception(string.Format(msg, args));
        }
    }
}
