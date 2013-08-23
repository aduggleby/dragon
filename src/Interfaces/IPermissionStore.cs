using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces
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
        bool IsRightInherited(Guid nodeID, Guid subjectID, string spec);

        IEnumerable<Guid> GetNodesWithRight(Guid subjectID, string spec);
        IEnumerable<IPermissionRight> GetRightsOnNodeWithInherited(Guid nodeID);

        IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> Tree { get; }
        IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> NodeListWithInheritedRights();

    }
}
