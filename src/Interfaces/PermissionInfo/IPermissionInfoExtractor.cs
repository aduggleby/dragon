using System;
using System.Collections.Generic;

namespace Dragon.Interfaces.PermissionInfo
{
    public interface IPermissionInfoExtractor
    {
        IEnumerable<IPermissionInfo> GetPermissionInfo(Guid node);
    }
}
