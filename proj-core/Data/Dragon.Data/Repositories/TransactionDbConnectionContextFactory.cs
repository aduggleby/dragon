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

        bool m_transactionAborted = false;
        DbConnection m_connection;
        DbTransaction m_transaction;

        public TransactionDbConnectionContextFactory(IConfiguration config, ILoggerFactory loggerFactory) :
            base(config, loggerFactory.CreateLogger<TransactionDbConnectionContextFactory>())
        {
            m_connection = CreateConnection();

            try
            {
                m_connection.Open();
                m_logger.LogTrace("Database connection successfull");

            }
            catch (Exception exInner)
            {
                m_logger.LogTrace($"Database connection failed: {exInner.Message}", exInner);
                throw;
            }

            m_transaction = m_connection.BeginTransaction();
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
                handle.Dispose();
                // Free any other managed objects here.
                if (!m_transactionAborted)
                {
                    m_transaction.Commit();
                }
                m_transaction.Dispose();
                m_connection.Dispose();
            }

            // Free any unmanaged objects here.
            //
            m_disposed = true;
        }

        public override T InDatabase<T>(Func<DbConnection, DbTransaction, T> db)
        {
            if (m_transactionAborted) throw new Exception("Transaction has already been aborted.");

            try
            {
                var res = db(m_connection, m_transaction);
                m_logger.LogTrace("InDatabase execution successful");

                return res;
            }
            catch (Exception ex)
            {
                m_transaction.Rollback();
                m_transactionAborted = true;
                m_logger.LogTrace("InDatabase execution failed", ex.ToString());
                throw;
            }
        }
    }

}
