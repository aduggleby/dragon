using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Common.Logging;
using Dragon.Core;
using Dragon.Core.Configuration;

namespace Dragon.SQL.Repositories
{
    public class ConnectionHelper
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLogger();

        public static Func<string,DbConnection> ConnectionFactory;
        public static Func<string, DbConnection> DefaultConnectionFactory;

        static ConnectionHelper()
        {
            DefaultConnectionFactory = (s) => new SqlConnection(s);
            ConnectionFactory = DefaultConnectionFactory;
        }

        public static DbConnection Open()
        {
            var conn = ConnectionFactory(ConnectionStringManager.Value);

            try
            {
                conn.Open();
                s_log.Trace(x => x("Database connection successfull"));
            }
            catch (Exception ex)
            {
                s_log.Error(x => x("Database connection failed"), ex);
                throw new Exception(Strings.SqlStores_Exception_ConnectFailed);
            }

            return conn;
        }
    }
}
