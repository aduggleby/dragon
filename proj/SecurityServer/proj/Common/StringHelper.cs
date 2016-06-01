using System;

namespace Dragon.SecurityServer.Common
{
    public static class StringHelper
    {
        public static string ReplaceLast(this string value, string oldValue, string newValue)
        {
            var index = value.LastIndexOf(oldValue, StringComparison.Ordinal);
            return index < 0 ? value : value.Remove(index, oldValue.Length).Insert(index, newValue);
        }
    }
}
