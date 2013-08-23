using System;

namespace Dragon.Interfaces.PermissionInfo
{
    public interface INameResolver
    {
        String Resolve(Guid ID);
    }
}
