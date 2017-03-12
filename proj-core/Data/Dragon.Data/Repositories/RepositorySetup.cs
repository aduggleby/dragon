using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Dragon.Data.Repositories
{
    public class RepositorySetup : IRepositorySetup
    {
        protected static readonly object m_existingTablesLock = new object();
        protected static List<Type> m_existingTables = new List<Type>();

        private readonly ILogger<RepositorySetup> m_logger;
        private IConnectionInstantiator m_connectionInstantiator;

        public RepositorySetup(IConnectionInstantiator connectionInstantiator, ILoggerFactory loggerFactory, IConfiguration config)
        {
            m_connectionInstantiator = connectionInstantiator;
            m_logger = loggerFactory.CreateLogger<RepositorySetup>();
        }

        public void EnsureTableExists<T>() where T : class
        {
            if (m_existingTables.Contains(typeof(T))) return;

            lock (m_existingTablesLock)
            {
                if (m_existingTables.Contains(typeof(T))) return;

                using (var c = m_connectionInstantiator.Open<T>())
                {
                    c.CreateTableIfNotExists<T>();
                    m_existingTables.Add(typeof(T));
                }
            }

        }

        public void DropTableIfExists<T>() where T : class
        {
            lock (m_existingTablesLock)
            {
                using (var c = m_connectionInstantiator.Open<T>())
                {
                    c.DropTableIfExists<T>();
                    m_existingTables.Remove(typeof(T));
                }
            }
        }

        public void DropTableIfExists(string name)
        {
            using (var c = m_connectionInstantiator.Open())
            {
                c.DropTableIfExists(name);
            }
        }
    }
}
