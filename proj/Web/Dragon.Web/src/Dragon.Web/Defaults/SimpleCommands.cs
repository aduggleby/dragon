using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Web.Attributes;
using Dragon.Web.Interfaces;

namespace Dragon.Web.Defaults
{
    public abstract class SimpleCommand<T> : CommandBase
        where T : TableBase
    {
        public Type Type { get { return typeof(T); } }

        public SimpleCommand()
        {
            Data = default(T);
        }

        public T Data { get; set; }


    }

    public class InsertCommandFor<T> : SimpleCommand<T>
        where T : TableBase
    {
        public override bool ExecutionAllowed(IContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class UpdateCommandFor<T> : SimpleCommand<T>
    where T : TableBase
    {
        public override bool ExecutionAllowed(IContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteCommandFor<T> : SimpleCommand<T>
    where T : TableBase
    {
        public override bool ExecutionAllowed(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
