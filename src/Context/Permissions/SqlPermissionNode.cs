using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Context.Permissions
{
    public class SqlPermissionNode : IPermissionNode
    {
        public Guid LID { get; set; }
        public Guid ParentID { get; set; }
        public Guid ChildID { get; set; }
    }
}
