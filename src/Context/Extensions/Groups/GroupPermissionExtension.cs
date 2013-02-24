using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Context.Extensions.Groups
{
    public static class GroupPermissionExtension
    {
        public const string RIGHT_MEMBEROF = "dragon.context.memberOf";

        public static void AddToGroup(this IPermissionStore permissionStore, Guid groupID, Guid subjectID)
        {
            permissionStore.AddRight(groupID, subjectID, RIGHT_MEMBEROF, false);
        }

        public static void RemoveFromGroup(this IPermissionStore permissionStore, Guid groupID, Guid subjectID)
        {
            permissionStore.RemoveRight(groupID, subjectID, RIGHT_MEMBEROF);
        }

        public static bool HasRightIncludingGroups(this IPermissionStore permissionStore, Guid nodeID, Guid subjectID,
                                            string spec)
        {
            return permissionStore.HasRight(nodeID,subjectID,spec) ||
                permissionStore.GetGroups(subjectID)
                .Any(groupID=>permissionStore.HasRight(nodeID,groupID,spec));   
        }

        public static IEnumerable<Guid> GetNodesWithRightIncludingGroups(this IPermissionStore permissionStore, 
            Guid subjectID, string spec)
        {
            return permissionStore.GetNodesWithRight(subjectID, spec).Union(
                permissionStore.GetGroups(subjectID).SelectMany(groupID=>
                    permissionStore.GetNodesWithRight(groupID, spec)));   
        }

        public static IEnumerable<Guid> GetGroups(this IPermissionStore permissionStore, Guid subjectID)
        {
            return permissionStore.GetNodesWithRight(subjectID, RIGHT_MEMBEROF);
        }
    }
}
