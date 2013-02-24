using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Context.Permissions
{
    public class SqlPermissionRight : IPermissionRight
    {
        public Guid LID { get; set; }
        public Guid NodeID { get; set; }
        public Guid SubjectID { get; set; }
        public string Spec { get; set; }
        public bool Inherit { get; set; }

    }
}
