using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.CPRX
{
    public class CommandExecutionResult
    {
        public CommandExecutionResult()
        {
            PipelineResults = new List<CommandPipelineResult>();
        }

        public bool Success
        {
            get
            {
                return PipelineResults.Any(x => !x.Success);
            }
        }

        public IList<CommandPipelineResult> PipelineResults { get; set; }
    }
}
