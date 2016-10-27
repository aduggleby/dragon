using System.Configuration;

namespace Dragon.Security.Hmac.Module.Configuration
{
    public class PathConfig : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public PathType Type
        {
            get { return (PathType)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("excludeParameters", IsRequired = false)]
        public string ExcludeParameters
        {
            get { return (string)this["excludeParameters"]; }
            set { this["excludeParameters"] = value; }
        }

        [ConfigurationProperty("ignoreBody", DefaultValue = true, IsRequired = false)]
        public bool IgnoreBody
        {
            get { return (bool)this["ignoreBody"]; }
            set { this["ignoreBody"] = value; }
        }

        public enum PathType
        {
            Include,
            Exclude
        }
    }
}
