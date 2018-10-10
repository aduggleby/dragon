using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dragon.Context.Util;

namespace System
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum e)
        {
            return ResourceUtil.GetEnumTextFromResource(e);
        }
    }
}