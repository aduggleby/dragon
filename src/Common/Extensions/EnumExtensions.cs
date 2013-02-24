using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dragon.Common.Attributes;
using Dragon.Common.Util;

namespace System
{
    public static class EnumExtensions
    {
        // Original from http://stackoverflow.com/questions/424366/c-sharp-string-enums

        public static string GetStringValue(this Enum enumeration, Type type)
        {
            FieldInfo fi = enumeration.GetType().GetField(enumeration.ToString());
            StringValue[] attributes = fi.GetCustomAttributes(type, false) as StringValue[];
            if (attributes.Length > 0)
            {
                return attributes[0].Value;
            }
            else
            {
                return null;
            }
        }

        public static string GetDisplayName(this Enum e)
        {
            return ResourceUtil.GetEnumTextFromResource(e);
        }
    }
}