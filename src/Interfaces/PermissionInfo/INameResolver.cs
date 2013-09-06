using System;

namespace Dragon.Interfaces.PermissionInfo
{
    public interface INameResolver
    {
        string ResolveSubjectID(Guid subjectID);
        string ResolveNodeID(Guid nodeID);
    }
}
