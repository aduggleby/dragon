using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context.Interfaces
{
    public interface IPermissionNode 
    {
        Guid ChildID { get; }
        Guid ParentID { get;  }
    }
}
