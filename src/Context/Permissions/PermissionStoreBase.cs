using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Common.Objects.Tree;
using Dragon.Interfaces;

namespace Dragon.Context.Permissions
{
    public abstract class PermissionStoreBase : IPermissionStore
    {
        private List<ITreeNode<Guid, List<IPermissionRight>>> m_treeInternal;
        private bool m_treeDirty = true;

        public PermissionStoreBase()
        {
        }

        public IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> Tree
        {
            get
            {
                if (m_treeDirty)
                {
                    RebuildTree();
                }
                return m_treeInternal;
            }
        }

        protected virtual void RebuildTree()
        {
            var rights = EnumerateAllRightsInternal();
            m_treeInternal = TreeBuilder.Build(
                EnumerateAllNodesInternal(),
                (x) => x.ParentID,
                (x) => x.ChildID,
                (x) => rights.Where(r => r.NodeID == x).ToList()).ToList();

            // add rights that are not in a tree structure
            foreach (var right in rights)
            {
                if (!(m_treeInternal.Any(x => x.Node.Equals(right.NodeID)) ||
                      m_treeInternal.Any(x => x.HasChildInTree(right.NodeID))))
                {
                    var node = new TreeNode<Guid, List<IPermissionRight>>()
                        {
                            Node = right.NodeID,
                            Data = new List<IPermissionRight>()
                        };
                    node.Data.Add(right);
                    m_treeInternal.Add(node);
                }
            }

            foreach (var node in m_treeInternal)
            {
                PushRights(node);
            }

            m_treeDirty = false;
        }

        public StringBuilder DebugOutputTree(
            StringBuilder sb = null,
            IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> nodes = null,
            int level = 0)
        {
            if (sb == null) sb = new StringBuilder();
            if (nodes==null) nodes = Tree;

            foreach (var node in nodes)
            {
                sb.Append(new string(' ', level));
                sb.Append("-");
                sb.Append(Guid4(node.Node));
                if (node.Data != null)
                {
                    foreach (var r in node.Data)
                    {
                        if (node.Data.First() != r) sb.Append(",");
                        sb.Append("(");
                        sb.Append(Guid4(r.SubjectID));
                        sb.Append("/");
                        sb.Append(r.Spec);
                        sb.Append("/");
                        sb.Append(r.Inherit ? "inherit" : "noinherit");
                        sb.Append(")");
                    }
                }
                sb.AppendLine();

                DebugOutputTree(sb, node.Children, level+1);
            }

            return sb;
        }
        
        private string Guid4(Guid g)
        {
            return g.ToString().Substring(0, 4);
        }

        private void PushRights(ITreeNode<Guid, List<IPermissionRight>> node)
        {
            if (node.Data == null) return;

            foreach (var permission in node.Data.Where(x => x.Inherit))
            {
                PushRight(permission, node.Children);
            }

            foreach (var child in node.Children)
            {
                PushRights(child);
            }
        }

        private void PushRight(IPermissionRight right, IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> nodes)
        {
            foreach (var child in nodes)
            {
                if (child.Data == null) child.Data = new List<IPermissionRight>();
                if (!child.Data.Any(x => x.LID.Equals(right.LID)))
                {
                    child.Data.Add(right);
                }
                PushRight(right, child.Children);
            }
        }

        private IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> AllNodes(
            IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> nodes = null)
        {
            if (nodes==null) nodes = Tree;

            foreach (var node in nodes)
            {
                yield return node;

                foreach (var child in AllNodes(node.Children))
                {
                    yield return child;
                }
            }
        }
        
        private ITreeNode<Guid, List<IPermissionRight>> GetNode(Guid nodeID)
        {
            return GetNode(nodeID, Tree);
        }

        private ITreeNode<Guid, List<IPermissionRight>> GetNode(Guid nodeID, IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> nodes)
        {
            var nsArray = nodes.ToArray();
            if (nsArray.Count() == 0) return null;
            return nsArray.FirstOrDefault(x => x.Node == nodeID) ?? GetNode(nodeID, nsArray.SelectMany(x => x.Children));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddNode(Guid parentID, Guid childID)
        {
            AddNodeInternal(parentID, childID);
            m_treeDirty = true;
        }
        
        public void RemoveNode(Guid parentID, Guid childID)
        {

            AddNodeInternal(parentID, childID);
            m_treeDirty = true;
        }

        protected abstract void AddNodeInternal(Guid parentID, Guid childID);
        protected abstract void RemoveNodeInternal(Guid parentID, Guid childID);
        
        protected abstract IEnumerable<Guid> EnumerateParentNodesInternal(Guid childID);
        protected abstract IEnumerable<Guid> EnumerateChildrenNodesInternal(Guid parentID);

        public virtual bool IsChildNodeOf(Guid parentID, Guid childID)
        {
            var parent = GetNode(parentID);
            return parent.HasChildInTree(childID);
        }

        protected abstract IEnumerable<IPermissionNode> EnumerateAllNodesInternal();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddRight(Guid nodeID, Guid subjectID, string spec, bool inherit)
        {
            AddRightInternal(nodeID, subjectID, spec, inherit);
            m_treeDirty = true;
        }
        
        public void RemoveRight(Guid nodeID, Guid subjectID, string spec)
        {
            RemoveRightInternal(nodeID, subjectID, spec);
            m_treeDirty = true;
        }
        
        protected abstract void AddRightInternal(Guid nodeID, Guid subjectID, string spec, bool inherit);
        protected abstract void RemoveRightInternal(Guid nodeID, Guid subjectID, string spec);
        
        protected abstract IEnumerable<IPermissionRight> EnumerateRightsInternal(Guid nodeID);
        protected abstract IEnumerable<IPermissionRight> EnumerateAllRightsInternal();

        public bool HasRight(Guid nodeID, Guid subjectID, string spec)
        {
            var node = GetNode(nodeID);

            if (node == null) return false;

            var right = node.Data.FirstOrDefault(x => x.SubjectID.Equals(subjectID) && x.Spec.Equals(spec));

            return (right != null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public IEnumerable<Guid> GetNodesWithRight(Guid subjectID, string spec)
        {
            List<Guid> guids = new List<Guid>();
            foreach (var node in AllNodes())
            {
                if (node.Data.Any(x => x.Spec.Equals(spec) && x.SubjectID.Equals(subjectID)))
                    guids.Add(node.Node);
            }

            return guids.Distinct();
        }

        public IEnumerable<IPermissionRight> GetRightsOnNodeWithInherited(Guid nodeID)
        {
            var candidate = AllNodes().FirstOrDefault(x => x.Node.Equals(nodeID));
            if (candidate == null) throw new Exception("Node not found.");
            return candidate.Data;
        }

        public IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> NodeListWithInheritedRights()
        {
            return AllNodes();
        }
    }
}
