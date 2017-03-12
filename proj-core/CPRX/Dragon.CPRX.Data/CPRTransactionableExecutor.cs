using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Dragon.Data.Repositories;

namespace Dragon.CPRX.Data
{
    public class CPRTransactionableExecutor : CPRExecutor
    {
        private ILoggerFactory m_loggerFactory;
        private IConfiguration m_config;

        public CPRTransactionableExecutor(ICPRStore store, ICPRContextFetcher ctxFetcher, IConfiguration config, ILoggerFactory loggerFactory)
            : base(store, ctxFetcher, config, loggerFactory.CreateLogger<CPRTransactionableExecutor>())
        {
            m_config = config;
            m_loggerFactory = loggerFactory;
        }
        
        protected override void ExecuteProjections<T>(T cmd, ICPRContext currentCtx)
        {
            var executedProjections = new List<ICPRProjection<T>>();
            try
            {
                using (var dbContextFactory = new TransactionDbConnectionContextFactory(m_config, m_loggerFactory))
                {
                    foreach (var projection in cmd.Projections)
                    {
                        executedProjections.Add(projection);

                        if (projection is ICPRDatabaseProjection<T>)
                        {
                            ((ICPRDatabaseProjection<T>)projection).Project(dbContextFactory, currentCtx, cmd, m_loggerFactory, m_config);
                        }
                        else
                        {
                            projection.Project(currentCtx, cmd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                executedProjections.Reverse();
                foreach (var projection in executedProjections)
                {
                    if (!(projection is ICPRDatabaseProjection<T>))
                    {
                        // for others that do not participate we unroll
                        projection.Unproject(currentCtx, cmd);
                    }
                }
                throw;
            }
        }
    }
}
