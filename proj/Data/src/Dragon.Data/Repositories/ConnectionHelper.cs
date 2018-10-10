using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Dragon.Data.Repositories
{
    public class ConnectionHelper
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLogger();

        public static Func<string, DbConnection> ConnectionFactory;
        public static Func<string, DbConnection> DefaultConnectionFactory;
        public static Func<Type, string> DefaultConnectionString = null;
        public static Func<Type, string> ConnectionString = null;

        static ConnectionHelper()
        {
            DefaultConnectionFactory = (s) => new SqlConnection(s);
            ConnectionFactory = DefaultConnectionFactory;
            DefaultConnectionString = (Type t) => ConnectionStringManager.Value;
            ConnectionString = DefaultConnectionString;
        }

        public static DbConnection Open()
        {
            return Open<object>();
        }

        public static DbConnection Open<T>()
        {
            var conn = ConnectionFactory(ConnectionString(typeof(T)));

            try
            {
                conn.Open();
                s_log.Trace(x => x("Database connection successfull"));
            }
            catch (Exception ex)
            {
                s_log.Error(x => x("Database connection failed"), ex);
                throw new Exception("Connection to database failed.", ex);
            }

            return conn;
        }
    }
}
