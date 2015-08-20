using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Utils
{
    public static class StringToBoolUtil
    {
        public static bool Interpret(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;

            s = s.ToLower();

            if (s == "on") return true;

            if (s.Length > 0) s = s.Substring(0, 1);

            return (s == "1" || s == "t");
        }
    }
}
