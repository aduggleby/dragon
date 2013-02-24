using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces
{
    public interface IPermissionRight
    {
        Guid LID { get; }
        Guid NodeID { get; }
        Guid SubjectID { get; }
        string Spec { get; }
        bool Inherit { get; }
    }
}
