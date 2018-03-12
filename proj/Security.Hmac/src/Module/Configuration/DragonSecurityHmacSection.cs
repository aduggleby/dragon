using System.Configuration;

namespace Dragon.Security.Hmac.Module.Configuration
{
    public class DragonSecurityHmacSection : ConfigurationSection
    {
        [ConfigurationProperty("serviceId", IsRequired = true)]
        public string ServiceId
        {
            get
            {
                return (string)this["serviceId"];
            }
            set
            {
                this["serviceId"] = value;
            }
        }

        [ConfigurationProperty("connectionStringName", DefaultValue = "", IsRequired = true)]
        public string ConnectionStringName
        {
            get
            {
                return (string)this["connectionStringName"];
            }
            set
            {
                this["connectionStringName"] = value;
            }
        }

        [ConfigurationProperty("usersTableName", DefaultValue = "DragonSecurityHmacUsers", IsRequired = true)]
        public string UsersTableName
        {
            get
            {
                return (string)this["usersTableName"];
            }
            set
            {
                this["usersTableName"] = value;
            }
        }

        [ConfigurationProperty("appsTableName", DefaultValue = "DragonSecurityHmacApps", IsRequired = true)]
        public string AppsTableName
        {
            get
            {
                return (string)this["appsTableName"];
            }
            set
            {
                this["appsTableName"] = value;
            }
        }

        [ConfigurationProperty("signatureParameterKey", DefaultValue = "signature", IsRequired = false)]
        public string SignatureParameterKey
        {
            get
            {
                return (string)this["signatureParameterKey"];
            }
            set
            {
                this["signatureParameterKey"] = value;
            }
        }

        [ConfigurationProperty("useHexEncoding", DefaultValue = false, IsRequired = false)]
        public bool UseHexEncoding
        {
            get
            {
                return (bool)this["useHexEncoding"];
            }
            set
            {
                this["useHexEncoding"] = value;
            }
        }

        [ConfigurationProperty("Paths", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(PathCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public PathCollection Paths
        {
            get
            {
                return (PathCollection)base["Paths"];
            }
            set
            {
                this["Paths"] = value;
            }
        }
    }
}