using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPRX
{
    public class CPRExecutor : ICPRExecutor
    {
        private ICPRContextFetcher m_ctxFetcher;

        public CPRExecutor(ICPRContextFetcher ctxFetcher)
        {
            m_ctxFetcher = ctxFetcher;
        }

        public CPRExecutionResult Execute(CPRCommand cmd)
        {
            var currentCtx = m_ctxFetcher.GetCurrentContext();

            cmd.Intercept(currentCtx);

            var result = new CPRExecutionResult();

            //
            // Security checks
            //
            result.AddErrors(cmd.SecurityValidators.SelectMany(v=>v.Authenticate(currentCtx, cmd)));
            if (result.Errors.Any())
            {
                // Security errors cause immediate abort.
                return result;
            }

            //
            // Validation and Business Results
            //
            result.AddErrors(cmd.Validators.SelectMany(v=>v.Validate(currentCtx, cmd)));
            if (result.Errors.Any())
            {
                return result;
            }

            //
            // Projections
            //
            using (var tx = new TransactionScope())
            {
                if (o.CommandID == Guid.Empty) o.CommandID = Guid.NewGuid();

                try
                {
                    foreach (var projection in cmd.Projections)
                    {
                        projection.Project(currentCtx, cmd);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            result.Success = true;
            
            return result;
        }
    }
}
