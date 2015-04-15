using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Web.Interfaces;

namespace Dragon.Web
{
    public abstract class CommandBase
    {
        public abstract bool ExecutionAllowed(IContext context);

        public bool HaltExecution { get; set;  }
    }
}
