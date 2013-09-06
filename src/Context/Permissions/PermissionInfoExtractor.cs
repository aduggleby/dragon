using System;
using System.Collections.Generic;
using System.Linq;
using Dragon.Interfaces;
using Dragon.Interfaces.PermissionInfo;

namespace Dragon.Context.Permissions
{
    public class PermissionInfoExtractor : IPermissionInfoExtractor
    {
        private readonly IPermissionStore _permissionStore;
        private readonly INameResolver _nameResolver;

        public PermissionInfoExtractor(IPermissionStore permissionStore, INameResolver nameResolver)
        {
            _permissionStore = permissionStore;
            _nameResolver = nameResolver;
        }

        public IEnumerable<IPermissionInfo> GetPermissionInfo(Guid node)
        {
            var permissions = _permissionStore.GetRightsOnNodeWithInherited(node).ToList();
            return permissions.Select(permission => new PermissionInfo
            {
                Subject = _nameResolver.Resolve(permission.SubjectID),
                Spec = permission.Spec,
                Inherit = permission.Inherit,
                Inherited = _permissionStore.IsRightInherited(node, permission.SubjectID, permission.Spec)
            });
        }
    }
}
