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
        protected ILogger m_logger;
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

        public virtual void BeforeExecute()
        {

        }

        public virtual void  AfterExecute()
        {

        }

        public virtual CPRExecutionResult Execute<T>(T cmd, bool ensureCommandsHaveSecurityValidators = true)
            where T : CPRCommand<T>
        {
            try
            {
                BeforeExecute();

                var currentCtx = m_ctxFetcher.GetCurrentContext();

                var interceptors = cmd.Interceptors.ToList();

                foreach (var interceptor in interceptors)
                {
                    interceptor.Intercept(currentCtx, cmd);
                }
                if (cmd is ICPRInterceptor<T>)
                {
                    ((ICPRInterceptor<T>)cmd).Intercept(currentCtx, cmd);
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
                // Security Validator will not be automatic (via is ICPRSecurityAValidator), since we want people to make a conscious decision
                // about which security policy applies
                if (result.Errors.Any())
                {
                    // Security errors cause immediate abort.
                    return result;
                }

                //
                // Validation and Business Results
                //
                ExecuteValidations(currentCtx, result, cmd);
                if (result.Errors.Any())
                {
                    return result;
                }

                DateTime utcStart = DateTime.UtcNow;
                if (cmd.ID == Guid.Empty) cmd.ID = Guid.NewGuid();

                try
                {
                    ExecuteProjections<T>(currentCtx, cmd);

                    m_store.Persist(cmd, currentCtx.UserID, utcStart);
                    result.Success = true;

                }
                catch(CPRValidationException cprEx)
                {
                    result.Success = false;
                    foreach (var err in cprEx.Errors)
                    {
                        result.AddError(err);
                    }
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
            finally
            {
                AfterExecute();
            }
        }

        protected virtual void ExecuteValidations<T>(ICPRContext currentCtx, CPRExecutionResult result, T cmd)
               where T : CPRCommand<T>
        {
            var validators = cmd.Validators.ToList();

            result.AddErrors(validators.SelectMany(v => v.Validate(currentCtx, cmd)));
            if (cmd is ICPRValidator<T>)
            {
                result.AddErrors(((ICPRValidator<T>)cmd).Validate(currentCtx, cmd));
            }

        }

        protected virtual void ExecuteProjections<T>(ICPRContext currentCtx, T cmd)
                 where T : CPRCommand<T>
        {
            var executedProjections = new List<ICPRProjection<T>>();
            var projections = cmd.Projections.ToList();
            try
            {
                if (projections.Any(x=> !(x is ICPRProjection<T>)))
                {
                    throw new Exception("Projections cannot derive from ICPRProjectionBase directly. This is just a marker interface. Use ICPRProjection<T> instead.");
                }

                var projectionCmd = cmd as ICPRProjection<T>;

                // If the class has specifically added this to their projections we respect their order
                if (projectionCmd != null && !projections.OfType<ICPRProjection<T>>().Contains(projectionCmd))
                {
                    projectionCmd.Project(currentCtx, cmd);
                }
                if (cmd is ICPRProjection<T>)
                {
                    ((ICPRProjection<T>)cmd).Project(currentCtx, cmd);
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
                    projection.Unproject(currentCtx, cmd);
                }
                throw;
            }
        }
    }
}
