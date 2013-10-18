using System.Configuration;
using Dragon.Common;

namespace Dragon.Core.Configuration
{
    public static class ConnectionStringManager
    {
        private static string m_connStr;
        private const string APPHARBOR_SQLSERVERCONNSTR = "SQLSERVER_CONNECTION_STRING";

        static ConnectionStringManager()
        {
            // AppHarbor Override
            var appHarborOverride = ConfigurationManager.AppSettings[APPHARBOR_SQLSERVERCONNSTR];

            if (appHarborOverride != null)
            {
                //
                // We are running on AppHarbor so change out connection strings
                //
                m_connStr = appHarborOverride;
            }
            else
            {
                var connStrEntry = ConfigurationManager.ConnectionStrings[Constants.DEFAULT_CONNECTIONSTRING_KEY];

                if (connStrEntry == null || string.IsNullOrWhiteSpace(connStrEntry.ConnectionString))
                {
                    throw Ex.For(Strings.SqlStores_Exception_ConnectionStringNotSet,
                                 Constants.DEFAULT_CONNECTIONSTRING_KEY);
                }

                m_connStr = connStrEntry.ConnectionString;
            }
        }

        public static string Value
        {
            get
            {
                return m_connStr;
            }
        }
    }
}
