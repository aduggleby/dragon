using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Dragon.Data.Repositories;
using System.Linq;

namespace Dragon.CPRX.Data
{
    public class CPRTransactionableExecutor : CPRExecutor
    {
        private ILoggerFactory m_loggerFactory;
        private IConfiguration m_config;
        private TransactionDbConnectionContextFactory m_dbContextFactory;
        private IDisposable m_scope;

        public CPRTransactionableExecutor(ICPRStore store, ICPRContextFetcher ctxFetcher, IConfiguration config, ILoggerFactory loggerFactory)
            : base(store, ctxFetcher, config, loggerFactory.CreateLogger<CPRTransactionableExecutor>())
        {
            m_config = config;
            m_loggerFactory = loggerFactory;
            m_logger = m_loggerFactory.CreateLogger<CPRTransactionableExecutor>();
        }

        public override void BeforeExecute()
        {
            m_scope = m_logger.BeginScope("Creating TransactionDbConnectionContextFactory as Execution Scope");
            m_dbContextFactory = new TransactionDbConnectionContextFactory(m_config, m_loggerFactory);
        }

        public override void AfterExecute()
        {
            m_dbContextFactory.Dispose();
            m_dbContextFactory = null;
            m_scope.Dispose();
            m_scope = null;
        }

        protected override void ExecuteValidations<T>(ICPRContext currentCtx, CPRExecutionResult result, T cmd)
        {
            base.ExecuteValidations(currentCtx, result, cmd);

            result.AddErrors(cmd.Validators.OfType<ICPRDatabaseValidator<T>>().SelectMany(v => v.Validate(
                m_dbContextFactory,
                currentCtx,
                cmd,
                m_loggerFactory,
                m_config)));

        }

        protected override void ExecuteProjections<T>(ICPRContext currentCtx, T cmd)
        {
            var executedProjections = new List<ICPRProjection<T>>();
            try
            {
                var projections = cmd.Projections.ToList();

                if (projections.Any(x => !(x is ICPRProjection<T> || x is ICPRDatabaseProjection<T>)))
                {
                    throw new Exception("Projections cannot derive from ICPRProjectionBase directly. This is just a marker interface. Use ICPRProjection<T> instead.");
                }

                var projectionCmd = cmd as ICPRProjection<T>;
                var dbProjectionCmd = cmd as ICPRDatabaseProjection<T>;

                // If the class has specifically added this to their projections we respect their order
                if (projectionCmd != null && !projections.OfType<ICPRProjection<T>>().Contains(projectionCmd))
                {
                    projectionCmd.Project(currentCtx, cmd);
                }
                if (dbProjectionCmd != null && !projections.OfType<ICPRDatabaseProjection<T>>().Contains(dbProjectionCmd))
                {
                    dbProjectionCmd.Project(m_dbContextFactory, currentCtx, cmd, m_loggerFactory, m_config);
                }
                foreach (var projection in projections.OfType<ICPRDatabaseProjection<T>>())
                {
                    projection.Project(m_dbContextFactory, currentCtx, cmd, m_loggerFactory, m_config);
                }
                foreach (var projection in projections.OfType<ICPRProjection<T>>())
                {
                    executedProjections.Add(projection);
                    projection.Project(currentCtx, cmd);
                }
            }
            catch (Exception ex)
            {
                executedProjections.Reverse();
                foreach (var projection in executedProjections)
                {
                    // for others that do not participate in transaction we unroll
                    projection.Unproject(currentCtx, cmd);
                }
                throw;
            }
        }
    }
}
