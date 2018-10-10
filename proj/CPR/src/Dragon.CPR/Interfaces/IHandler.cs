using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Errors;

namespace Dragon.CPR.Interfaces
{
    public interface IHandler<TCommand>
        where TCommand : CommandBase
    {
        IEnumerable<ErrorBase> Handle(TCommand obj);

        int Order { get; }
    }
}
