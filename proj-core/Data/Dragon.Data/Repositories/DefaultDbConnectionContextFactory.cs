using Dragon.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Dragon.Data.Repositories
{
    public class DefaultDbConnectionContextFactory : IDbConnectionContextFactory
    {
        protected readonly ILogger m_logger;
        protected IConfiguration m_config;

        protected bool m_verboseLoggingEnabled = false;

        public DefaultDbConnectionContextFactory(IConfiguration config, ILoggerFactory loggerFactory)
            : this(config, loggerFactory.CreateLogger<DefaultDbConnectionContextFactory>())
        {
            if (Environment.GetEnvironmentVariable("Dragon.Data.Verbose") == "1")
            {
                m_verboseLoggingEnabled = true;
            }
        }

        protected DefaultDbConnectionContextFactory(IConfiguration config, ILogger logger)
        {
            m_config = config;
            m_logger = logger;

        }
        public virtual void InDatabase(Action<DbConnection, DbTransaction> db)
        {
            InDatabase<object>((c,t) =>
            {
                db(c, t);
                return null;
            });
        }

        public virtual T InDatabase<T>(Func<DbConnection, DbTransaction, T> db)
        {
            var conn = CreateConnection();

            try
            {
                try
                {
                    conn.Open();
                    if(m_verboseLoggingEnabled) m_logger.LogDebug("Database connection successfull");
                }
                catch (Exception exInner)
                {
                    m_logger.LogWarning("Database connection failed", exInner);
                    if (conn != null) conn.Dispose();
                    throw;
                }

                if (m_verboseLoggingEnabled) m_logger.LogTrace("Database connection successfull");

                var res = db(conn, null);
                if (m_verboseLoggingEnabled) m_logger.LogTrace("InDatabase executed successfully");

                return res;
            }
            catch (Exception ex)
            {
                m_logger.LogWarning("InDatabase execution failed");
                m_logger.LogError(ex.ToString());
                m_logger.LogError(ex.StackTrace);
                throw new Exception("InDatabase execution fail.", ex);
            }
            finally
            {
                conn.Close();
                if (m_verboseLoggingEnabled) m_logger.LogDebug("Database connection closed.");
            }
        }

        protected virtual DbConnection CreateConnection()
        {
            string connectionString = ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("ConnectionString 'Dragon' of app settings 'Dragon:Data:ConnectionString' not set.");
            }

            if (m_verboseLoggingEnabled) m_logger.LogTrace("Dragon.Data ready with connection string: " + connectionString);

            var conn = new SqlConnection(connectionString);
            return conn;
        }

        protected virtual string ConnectionString
        {
            get
            {
                return m_config.GetConnectionString("Dragon") ?? m_config["Dragon:Data:ConnectionString"];
            }
        }
    }
}
