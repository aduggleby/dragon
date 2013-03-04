using System;
using System.Collections.Generic;
using System.Linq;
using Dragon.Context.Permissions;
using Dragon.Interfaces;

namespace Dragon.Tests.PermissionStore.Helpers
{
    public class InMemoryPermissionStore : PermissionStoreBase
    {
        private List<Right> m_rights = new List<Right>();
        private List<Node> m_nodes = new List<Node>();

        public InMemoryPermissionStore()
        {
            base.RebuildTree();
        }

        protected override void AddNodeInternal(Guid parentID, Guid childID)
        {
            if (!EnumerateParentNodesInternal(childID).Contains(parentID))
            {
                m_nodes.Add(new Node()
                {
                    LID = Guid.NewGuid(),
                    ParentID = parentID,
                    ChildID = childID
                });
            }

            //base.AddNode(parentID, childID);
        }

        protected override void RemoveNodeInternal(Guid parentID, Guid childID)
        {
            if (EnumerateParentNodesInternal(childID).Contains(parentID))
            {
                m_nodes.Remove(m_nodes.FirstOrDefault(x => x.ParentID.Equals(parentID) && x.ChildID.Equals(childID)));
            }

            //base.RemoveNode(parentID, childID);
        }

        protected override IEnumerable<Guid> EnumerateParentNodesInternal(Guid childID)
        {
            return m_nodes.Where(x => x.ChildID.Equals(childID)).Select(x => x.ParentID);
        }

        protected override IEnumerable<Guid> EnumerateChildrenNodesInternal(Guid parentID)
        {
            return m_nodes.Where(x => x.ParentID.Equals(parentID)).Select(x => x.ChildID);
        }

        protected override IEnumerable<IPermissionNode> EnumerateAllNodesInternal()
        {
            // clone
            return m_nodes.Select(x => new Node() { LID = x.LID, ParentID = x.ParentID, ChildID = x.ChildID }).ToArray();
        }

        protected override void AddRightInternal(Guid nodeID, Guid subjectID, string spec, bool inherit)
        {
            RemoveRight(nodeID, subjectID, spec, false);

            m_rights.Add(new Right()
            {
                LID = Guid.NewGuid(),
                NodeID = nodeID,
                SubjectID = subjectID,
                Spec = spec,
                Inherit = inherit
            });

            //base.AddRight(nodeID, subjectID, spec, inherit);
        }

        protected override void RemoveRightInternal(Guid nodeID, Guid subjectID, string spec)
        {
            RemoveRight(nodeID, subjectID, spec, true);
        }

        public void RemoveRight(Guid nodeID, Guid subjectID, string spec, bool callBase)
        {
            if (HasRight(nodeID, subjectID, spec))
            {
                var candidate =
                    m_rights.FirstOrDefault(
                        x =>
                        x.NodeID.Equals(nodeID) && x.SubjectID.Equals(subjectID) &&
                        x.Spec.Equals(spec, StringComparison.CurrentCultureIgnoreCase));

                if (candidate != null)
                {
                    m_rights.Remove(candidate);
                }
            }

            if (callBase)
            {
                //base.RemoveRight(nodeID, subjectID, spec);
            }
        }

        protected override IEnumerable<IPermissionRight> EnumerateRightsInternal(Guid nodeID)
        {
            // clone
            return m_rights.Where(x => x.NodeID.Equals(nodeID)).Select(x => new Right()
                {
                    LID = x.LID,
                    NodeID = x.NodeID,
                    SubjectID = x.SubjectID,
                    Spec = x.Spec,
                    Inherit = x.Inherit
                }).ToArray();
        }

        protected override IEnumerable<IPermissionRight> EnumerateAllRightsInternal()
        {
            // clone
            return m_rights.Select(x => new Right()
            {
                LID = x.LID,
                NodeID = x.NodeID,
                SubjectID = x.SubjectID,
                Spec = x.Spec,
                Inherit = x.Inherit
            }).ToArray();
        }

        private class Right : IPermissionRight
        {
            public Guid LID { get; set; }
            public Guid NodeID { get; set; }
            public Guid SubjectID { get; set; }
            public string Spec { get; set; }
            public bool Inherit { get; set; }
        }

        private class Node : IPermissionNode
        {
            public Guid LID { get; set; }
            public Guid ParentID { get; set; }
            public Guid ChildID { get; set; }
        }
    }
}
