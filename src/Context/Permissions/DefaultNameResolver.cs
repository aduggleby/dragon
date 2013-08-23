using System;
using Dragon.Interfaces.PermissionInfo;

namespace Dragon.Context.Permissions
{
    public class DefaultNameResolver : INameResolver
    {
        private readonly string _prefix;

        public DefaultNameResolver(String prefix)
        {
            _prefix = prefix;
        }

        public string Resolve(Guid ID)
        {
            return _prefix + ID;
        }
    }
}