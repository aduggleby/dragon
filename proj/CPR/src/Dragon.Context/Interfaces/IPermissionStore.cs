using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context.Interfaces
{
    public interface IPermissionStore
    {
        void AddNode(Guid parentID, Guid childID);
        void RemoveNode(Guid parentID, Guid childID);
        bool IsChildNodeOf(Guid parentID, Guid childID);
        bool IsDirectChildNodeOf(Guid parentID, Guid childID);
        
        void AddRight(Guid nodeID, Guid subjectID, string spec, bool inherit);
        void RemoveRight(Guid nodeID, Guid subjectID, string spec);
        bool HasRight(Guid nodeID, Guid subjectID, string spec);
        Guid? NodeRightIsInheritedFrom(Guid nodeID, Guid subjectID, string spec);

        bool HasNode(Guid nodeID);

        IEnumerable<Guid> GetNodesWithRight(Guid subjectID, string spec);
        IEnumerable<IPermissionRight> GetRightsOnNodeWithInherited(Guid nodeID);
        Dictionary<Guid, List<IPermissionRight>> GetNodesSubjectHasRightsOn(Guid subjectID);


        IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> Tree { get; }
        IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> NodeListWithInheritedRights();

    }
}
