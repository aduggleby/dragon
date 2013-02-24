using System;

namespace System 
{
    public static class ReflectionExtensions {
        public static bool PropertyExists(this Type type, string propertyName) {
            if (type == null || propertyName == null) {
                return false;
            }
            return type.GetProperty(propertyName) != null;
        }
    }
}
