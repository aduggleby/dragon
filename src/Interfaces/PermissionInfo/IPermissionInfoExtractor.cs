using System;
using System.Collections.Generic;

namespace Dragon.Interfaces.PermissionInfo
{
    public interface IPermissionInfoExtractor
    {
        IEnumerable<IPermissionInfo> GetPermissionInfoForNode(Guid nodeID);
        IEnumerable<IPermissionInfo> GetPermissionInfoForSubject(Guid subjectID);
    }
}
