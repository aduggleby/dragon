using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dragon.CPR.Interfaces;

namespace Dragon.CPR.Impl
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private static string m_connStr;

        private const string APPHARBOR_SQLSERVERCONNSTR = "SQLSERVER_CONNECTION_STRING";

        private const string DRAGON_CONNSTR_KEY = "Dragon";

        static SqlConnectionFactory()
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
                var connStrEntry = ConfigurationManager.ConnectionStrings[DRAGON_CONNSTR_KEY];

                if (connStrEntry == null || string.IsNullOrWhiteSpace(connStrEntry.ConnectionString))
                {
                    throw new Exception("Connection string '" + DRAGON_CONNSTR_KEY + "' not found.");
                }

                m_connStr = connStrEntry.ConnectionString;
            }
        }

        public SqlConnection New(Type asking)
        {
            return new SqlConnection(m_connStr);
        }
    }
}
