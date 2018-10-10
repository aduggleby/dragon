using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.CPRX
{
    public class CommandSecurityPolicyPipelineResult : CommandPipelineResult
    {
        public CommandSecurityPolicyPipelineResult(CommandSecurityPolicyPipelineResultType type, object owner):base(owner)
        {
            Type = type;
        }

        public override bool Success
        {
            get
            {
                return Type == CommandSecurityPolicyPipelineResultType.NoObjection;
            }

          
        }

        public override string Message
        {
            get
            {
                return Type.ToString();
            }

        }
        
        public CommandSecurityPolicyPipelineResultType Type { get; private set; }
    }
}
