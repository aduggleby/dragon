using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Common.Attributes.Data;
using Dragon.Interfaces;

namespace Dragon.Context.Permissions
{
    public class DragonPermissionNode : IPermissionNode, IDragonTable
    {
        [Key]
        public Guid LID { get; set; }
        public Guid ParentID { get; set; }
        public Guid ChildID { get; set; }
    }
}
