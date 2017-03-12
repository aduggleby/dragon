using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dragon.CPRX
{
    public class CommandExecutor
    {
        public CommandExecutionResult Execute(CommandBase cmd)
        {
            var executionResult = new CommandExecutionResult();
            executionResult.PipelineResults = new List<CommandPipelineResult>();

            // SECURITY
            var securityResult = cmd.SecurityPolicy.Validate(cmd);
            var securityPipelineResult = new CommandSecurityPolicyPipelineResult(securityResult, cmd.SecurityPolicy);
            executionResult.PipelineResults.Add(securityPipelineResult);

            if (!executionResult.Success) return executionResult;

            // VALIDATORS


            if (!executionResult.Success) return executionResult;

            // PROJECTIONS
            using (var tx = new TransactionScope())
            {
                tx.Commit();
            }

            return executionResult;
        }
    }
}
