using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPRX
{
    public class CPRExecutor : ICPRExecutor
    {
        private ICPRContextFetcher m_ctxFetcher;
        protected readonly ILogger m_logger;
        protected readonly IConfiguration m_config;
        protected readonly ICPRStore m_store;

        public CPRExecutor(ICPRStore store, ICPRContextFetcher ctxFetcher, IConfiguration config, ILoggerFactory loggerFactory)
            : this(store, ctxFetcher, config, loggerFactory.CreateLogger<CPRExecutor>())
        {

        }

        public CPRExecutor(ICPRStore store, ICPRContextFetcher ctxFetcher, IConfiguration config, ILogger logger)
        {
            m_store = store;
            m_config = config;
            m_ctxFetcher = ctxFetcher;
            m_logger = logger;
        }

        public virtual CPRExecutionResult Execute<T>(T cmd, bool ensureCommandsHaveSecurityValidators = true)
            where T : CPRCommand<T>
        {
            var currentCtx = m_ctxFetcher.GetCurrentContext();

            foreach (var interceptor in cmd.Interceptors)
            {
                interceptor.Intercept(currentCtx, cmd);
            }

            var result = new CPRExecutionResult();

            //
            // Security checks
            //
            if (ensureCommandsHaveSecurityValidators && !cmd.SecurityValidators.Any())
            {
                throw new Exception("Command does not have at least on security validator");
            }

            result.AddErrors(cmd.SecurityValidators.SelectMany(v => v.Authenticate(currentCtx, cmd)));
            if (result.Errors.Any())
            {
                // Security errors cause immediate abort.
                return result;
            }

            //
            // Validation and Business Results
            //
            result.AddErrors(cmd.Validators.SelectMany(v => v.Validate(currentCtx, cmd)));
            if (result.Errors.Any())
            {
                return result;
            }

            DateTime utcStart = DateTime.UtcNow;
            if (cmd.ID == Guid.Empty) cmd.ID = Guid.NewGuid();

            try
            {
                ExecuteProjections<T>(cmd, currentCtx);

                m_store.Persist(cmd, currentCtx.UserID, utcStart);
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(new CPRError()
                {
                    Message = "Exception during execution: " + ex.Message
                });
            }

            return result;
        }

        protected virtual void ExecuteProjections<T>(T cmd, ICPRContext currentCtx)
                 where T : CPRCommand<T>
        {
            var executedProjections = new List<ICPRProjection<T>>();
            try
            {
                foreach (var projection in cmd.Projections)
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
                    projection.Unproject(currentCtx, cmd);
                }
                throw;
            }
        }
    }
}
