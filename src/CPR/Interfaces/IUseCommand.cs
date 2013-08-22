using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Interfaces
{
    public  interface IUseCommand<TCommand> 
    {
        TCommand Command { get; set; }
    }
}
