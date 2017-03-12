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
    public class DefaultConnectionInstantiator : IConnectionInstantiator
    {
        private readonly ILogger<DefaultConnectionInstantiator> m_logger;
        private IConfiguration m_config;

        public DefaultConnectionInstantiator(IConfiguration config, ILoggerFactory loggerFactory)
        {
            m_config = config;
            m_logger = loggerFactory.CreateLogger<DefaultConnectionInstantiator>();
        }

        public DbConnection Open()
        {
            return Open<object>();
        }

        public DbConnection Open<T>()
        {
            var connectionString = m_config.GetConnectionString("Dragon") ?? m_config["Dragon:Data:ConnectionString"];

            var conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                m_logger.LogTrace("Database connection successfull");
            }
            catch (Exception ex)
            {
                m_logger.LogTrace("Database connection failed", ex);
                throw new Exception("Connection to database failed.", ex);
            }

            return conn;
        }
    }
}
