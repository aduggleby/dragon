using System.Configuration;
using Dragon.Common;

namespace Dragon.Context
{
    public static class StandardSqlStore
    {
        private static string m_connStr;
        private const string APPHARBOR_SQLSERVERCONNSTR = "SQLSERVER_CONNECTION_STRING";

        static StandardSqlStore()
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
                    throw Ex.For(SQL.SqlStores_Exception_ConnectionStringNotSet,
                                 Constants.DEFAULT_CONNECTIONSTRING_KEY);
                }

                m_connStr = connStrEntry.ConnectionString;
            }
        }

        public static string ConnectionString 
        {
            get
            {
                return m_connStr;
            }
        }
    }
}
