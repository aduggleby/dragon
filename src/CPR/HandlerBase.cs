using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Errors;
using Dragon.CPR.Interfaces;

namespace Dragon.CPR
{
    public abstract class HandlerBase<T> : IHandler<T>
        where T : CommandBase
    {
        public IReadRepository Repository { get; set; }

        public abstract IEnumerable<ErrorBase> Handle(T obj);

        public virtual int Order
        {
            get { return 100; }
        }
    }
}
