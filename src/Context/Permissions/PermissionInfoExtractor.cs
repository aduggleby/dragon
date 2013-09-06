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


        public IEnumerable<IPermissionInfo> GetPermissionInfoForNode(Guid nodeID)
        {
            var permissions = _permissionStore.GetRightsOnNodeWithInherited(nodeID).ToList();
            return permissions.Select(permission => new PermissionInfo
            {
                DisplayName = _nameResolver.ResolveSubjectID(permission.SubjectID),
                Spec = permission.Spec + " (" + permission.LID.ToString().Substring(0, 4) + ")",
                Inherit = permission.Inherit,
                InheritedFrom = ResolveNodeID(_permissionStore.NodeRightIsInheritedFrom(
                    nodeID /* test the node it inherits to */, 
                    permission.SubjectID, 
                    permission.Spec))
            });
           
        }

           private string ResolveSubjectID(Guid? subjectID)
        {
            if (!subjectID.HasValue) return null;
            return _nameResolver.ResolveSubjectID(subjectID.Value);
        }

        private string ResolveNodeID(Guid? nodeID)
        {
            if (!nodeID.HasValue) return null;
            return _nameResolver.ResolveNodeID(nodeID.Value);
        }

        public IEnumerable<IPermissionInfo> GetPermissionInfoForSubject(Guid subjectID)
        {
            var permissions = _permissionStore.GetNodesSubjectHasRightsOn(subjectID).ToList();
            return permissions.SelectMany(node => 
                node.Value.Select(permission=> new PermissionInfo
            {
                DisplayName = _nameResolver.ResolveNodeID(node.Key),
                Spec = permission.Spec + " (" + permission.LID.ToString().Substring(0,4) + ")",
                Inherit = permission.Inherit,
                InheritedFrom = ResolveNodeID(_permissionStore.NodeRightIsInheritedFrom(
                    node.Key /* test the node we are in */, 
                    permission.SubjectID, 
                    permission.Spec))
            }));
        }

    }
}
