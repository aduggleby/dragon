using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web.Mvc;

namespace Dragon.Context.Util
{
    public static class ResourceUtil
    {
        public static string RESXKEY_DISPLAYNAME = "Model_{0}_{1}";

        public static string RESXKEY_GROUP = "Model_{0}_Group_{1}";
        public static string RESXKEY_GROUP_HELPBLOCK = "Model_{0}_Group_{1}_Helptext";

        public static string RESXKEY_HELPBLOCK = "Model_{0}_{1}_HelpBlock";
        public static string RESXKEY_ADDONTEXT = "Model_{0}_{1}_AddOnText";
        public static string RESXKEY_HELPINLINE = "Model_{0}_{1}_HelpInline";
        public static string RESXKEY_HELPTOOLTIP = "Model_{0}_{1}_HelpTooltip";

        public static string RESXKEY_ENUM = "Enum_{0}_{1}";

        public static ResourceManager StringResourceManager;

        public static string GetTextFromResource(string keyFormat, ModelMetadata metaInfo)
        {
            return StringResourceManager.GetString(string.Format(keyFormat, metaInfo.ContainerType.Name, metaInfo.PropertyName));
        }

        public static string GetTextFromResource(string keyFormat, ModelMetadata metaInfo, string additionalKey)
        {
            return StringResourceManager.GetString(string.Format(keyFormat, metaInfo.ContainerType.Name, additionalKey));
        }

        public static string GetTextFromResource(string keyFormat, PropertyInfo pi)
        {
            return StringResourceManager.GetString(string.Format(keyFormat, pi.DeclaringType.Name, pi.Name));
        }

        public static string GetTextFromResource(string keyFormat, params string[] s)
        {
            return StringResourceManager.GetString(string.Format(keyFormat, s));
        }

        public static string GetEnumTextFromResource(Enum e)
        {
            var lookup = string.Format(RESXKEY_ENUM, e.GetType().Name, Enum.GetName(e.GetType(), e));
            return StringResourceManager.GetString(lookup);
        }
    }
}
