using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dragon.Common.Objects.Tree;
using Dragon.Context.Exceptions;
using Dragon.Interfaces;

namespace Dragon.Context.Permissions
{
    public abstract class PermissionStoreBase : StoreBase, IPermissionStore
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
                    DebugPrint("Rebuilding tree...");
                    DebugPrint(DebugOutputTree(null, m_treeInternal).ToString());
                    DebugPrint("... now ...");
                    RebuildTree();
                    DebugPrint(DebugOutputTree(null, m_treeInternal).ToString());
                    DebugPrint("...done");

                }
                else
                {
                    DebugPrint("Tree is not dirty.");
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

            DebugPrint("Tree built internally...");
            DebugPrint(DebugOutputTree(null, m_treeInternal).ToString());
            DebugPrint("...now adding loose rights");

            // add rights that are not in a tree structure
            foreach (var right in rights)
            {
                var exNode = m_treeInternal
                    .Select(x => x.GetChildInTree(right.NodeID))
                    .FirstOrDefault(x => x != null);

                if (exNode == null)
                {
                    var node = new TreeNode<Guid, List<IPermissionRight>>()
                        {
                            Node = right.NodeID,
                            Data = new List<IPermissionRight>()
                        };
                    DebugPrint("Node '{0}' not found, so adding with right: {1}", right.NodeID, Dump(right));
                    node.Data.Add(right);
                    m_treeInternal.Add(node);
                }

                else
                {

                    if (!exNode.Data.Any(x => x.SubjectID.Equals(right.SubjectID) && x.Spec.Equals(right.Spec)))
                    {
                        DebugPrint("Node '{0}' found, so adding a right: {1}", right.NodeID, Dump(right));
                        exNode.Data.Add(right);
                    }
                    else
                    {
                        DebugPrint("Node '{0}' found, but right already exists: {1}", right.NodeID, Dump(right));
                    }
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
            if (nodes == null) nodes = Tree;

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

                DebugOutputTree(sb, node.Children, level + 1);
            }

            return sb;
        }

        private string Guid4(Guid g)
        {
            return g.ToString().Substring(0, 4);
        }

        private void DebugPrint(string msg, params object[] p)
        {
            // Debug.WriteLine(string.Format(msg, p));
        }

        private string Dump(IPermissionRight r)
        {
            return string.Format("N:{0},S:{1},R:{2},I:{3}", r.NodeID, r.SubjectID, r.Spec, r.Inherit);
        }

        private void PushRights(ITreeNode<Guid, List<IPermissionRight>> node)
        {
            if (node.Data != null)
            {
                foreach (var permission in node.Data.Where(x => x.Inherit))
                {
                    PushRight(permission, node.Children);
                }

                foreach (var child in node.Children)
                {
                    PushRights(child);
                }
            }
        }

        private void PushRight(IPermissionRight right, IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> nodes)
        {
            foreach (var child in nodes)
            {

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
            if (nodes == null) nodes = Tree;

            foreach (var node in nodes)
            {
                yield return node;

                foreach (var child in AllNodes(node.Children))
                {
                    yield return child;
                }
            }
        }

        private IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> AllUniqueNodes()
        {
            var visited = new List<Guid>();
            foreach (var node in AllNodes())
            {
                if (!visited.Contains(node.Node))
                {
                    visited.Add(node.Node);
                    yield return node;
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
            DebugPrint("AddNode P: {0} C: {1}", parentID, childID);

            AddNodeInternal(parentID, childID);
            m_treeDirty = true;
        }

        public void RemoveNode(Guid parentID, Guid childID)
        {
            DebugPrint("RemoveNode P: {0} C: {1}", parentID, childID);

            if (!IsChildNodeOf(parentID, childID))
                throw new NodeDoesNotExistException();

            EnumerateAllRightsInternal()
                .Where(x => x.NodeID.Equals(childID))
                .ToList()
                .ForEach(x => RemoveRight(x.NodeID, x.SubjectID, x.Spec));

            RemoveNodeInternal(parentID, childID);
            m_treeDirty = true;
        }

        protected abstract void AddNodeInternal(Guid parentID, Guid childID);
        protected abstract void RemoveNodeInternal(Guid parentID, Guid childID);

        protected abstract IEnumerable<Guid> EnumerateParentNodesInternal(Guid childID);
        protected abstract IEnumerable<Guid> EnumerateChildrenNodesInternal(Guid parentID);

        public virtual bool IsDirectChildNodeOf(Guid parentID, Guid childID)
        {
            var parent = GetNode(parentID);
            if (parent == null) return false;
            return parent.Children.Any(x => x.Node.Equals(childID));
        }

        public virtual bool IsChildNodeOf(Guid parentID, Guid childID)
        {
            var parent = GetNode(parentID);
            if (parent == null) return false;
            return parent.HasChildInTree(childID);
        }

        protected abstract IEnumerable<IPermissionNode> EnumerateAllNodesInternal();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddRight(Guid nodeID, Guid subjectID, string spec, bool inherit)
        {
            DebugPrint("AddRight N: {0} S: {1} R: {2} I: {3}", nodeID, subjectID, spec, inherit);

            AddRightInternal(nodeID, subjectID, spec, inherit);
            m_treeDirty = true;
        }

        public void RemoveRight(Guid nodeID, Guid subjectID, string spec)
        {
            if (!HasRight(nodeID, subjectID, spec)) throw new RightDoesNotExistException();

            DebugPrint("RemoveRight N: {0} S: {1} R: {2}", nodeID, subjectID, spec);

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

            if (node == null)
            {
                DebugPrint("HasRight - Node not found. - N: {0} S: {1} R: {2}", nodeID, subjectID, spec);
                return false;
            }

            var right = node.Data.FirstOrDefault(x => x.SubjectID.Equals(subjectID) && x.Spec.Equals(spec));
            if (right == null)
            {
                DebugPrint("HasRight - Right not found. - N: {0} S: {1} R: {2}", nodeID, subjectID, spec);
            }

            return (right != null);
        }

        public Guid? NodeRightIsInheritedFrom(Guid nodeID, Guid subjectID, string spec)
        {
            var node = GetNode(nodeID);

            var right = node.Data.FirstOrDefault(x => x.SubjectID.Equals(subjectID) && x.Spec.Equals(spec));

            if (right == null) return (Guid?)null;

            return !right.NodeID.Equals(node.Node) ? right.NodeID : (Guid?)null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Dictionary<Guid, List<IPermissionRight>> GetNodesSubjectHasRightsOn(Guid subjectID)
        {
            Debug.WriteLine(DebugOutputTree().ToString());

            var nodes = new Dictionary<Guid, List<IPermissionRight>>();
            foreach (var node in AllUniqueNodes())
            {
                var rightsOfThisUse = node.Data.Where(x => x.SubjectID.Equals(subjectID));
                foreach (var right in rightsOfThisUse )
                {
                    Debug.WriteLine(node.Node + ": LID " + right.LID);
                    if (!nodes.ContainsKey(node.Node))
                    {
                        nodes.Add(node.Node, new List<IPermissionRight>());
                    }
                    nodes[node.Node].Add(right);
                }
            }

            return nodes;
        }

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


        public bool HasNode(Guid nodeID)
        {
            var candidate = AllNodes().FirstOrDefault(x => x.Node.Equals(nodeID));
            return (candidate != null);
        }


        public IEnumerable<IPermissionRight> GetRightsOnNodeWithInherited(Guid nodeID)
        {
            var candidate = AllNodes().FirstOrDefault(x => x.Node.Equals(nodeID));
            if (candidate == null) return new List<IPermissionRight>();
            return candidate.Data;
        }

        public IEnumerable<ITreeNode<Guid, List<IPermissionRight>>> NodeListWithInheritedRights()
        {
            return AllNodes();
        }
    }
}
