using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.CPRX
{
    public class CommandPipelineResult
    {
        public CommandPipelineResult(object owner)
        {
            Step = owner.GetType().Name;
        }

        public CommandPipelineResult(bool success, object owner):this(owner)
        {
            Success = success;
        }

        public CommandPipelineResult(bool success, string message, object owner) : this(success, owner)
        {
            Message = message;
        }

        public virtual bool Success { get; private set; }

        public virtual string Step { get; private set; }

        public virtual string Message { get; private set; }
    }
}
