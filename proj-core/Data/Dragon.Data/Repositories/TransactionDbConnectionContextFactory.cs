using Dragon.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;

namespace Dragon.Data.Repositories
{
    public class TransactionDbConnectionContextFactory : DefaultDbConnectionContextFactory, IDisposable
    {
        // Flag: Has Dispose already been called?
        bool m_disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public static volatile int OpenCount = 0;

        bool m_transactionAborted = false;
        DbConnection m_connection;
        DbTransaction m_transaction;

        public TransactionDbConnectionContextFactory(IConfiguration config, ILoggerFactory loggerFactory) :
            base(config, loggerFactory.CreateLogger<TransactionDbConnectionContextFactory>())
        {
            m_connection = CreateConnection();

            try
            {
                OpenCount++;
                m_connection.Open();
                if (m_verboseLoggingEnabled) m_logger.LogDebug($"Database connection successfull. OpenCount: {OpenCount}");

            }
            catch (Exception exInner)
            {
                OpenCount--;
                m_logger.LogWarning($"Database connection failed: {exInner.Message}", exInner);
                if (m_connection != null)
                {
                    m_connection.Dispose();
                }
                throw;
            }

            m_transaction = m_connection.BeginTransaction();
        }

        ~TransactionDbConnectionContextFactory()
        {
            Dispose(false);
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                if (m_verboseLoggingEnabled) m_logger.LogDebug($"Disposing any open transaction and connection. OpenCount: {OpenCount}");

                handle.Dispose();
                // Free any other managed objects
                if (!m_transactionAborted)
                {
                    m_transaction.Commit();
                }
                OpenCount--;
                m_transaction.Dispose();
                m_connection.Dispose();
                m_transaction = null;
                m_connection = null;
            }

            // Free any unmanaged objects
            m_disposed = true;
        }

        public override T InDatabase<T>(Func<DbConnection, DbTransaction, T> db)
        {
            if (m_transactionAborted) throw new Exception("Transaction has already been aborted.");

            try
            {
                var res = db(m_connection, m_transaction);
                if (m_verboseLoggingEnabled) m_logger.LogTrace("InDatabase execution successful");

                return res;
            }
            catch (Exception ex)
            {
                m_transaction.Rollback();
                m_transactionAborted = true;
                m_logger.LogWarning("InDatabase execution failed", ex.ToString());
                throw;
            }
        }
    }

}
