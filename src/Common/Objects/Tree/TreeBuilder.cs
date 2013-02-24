using System;
using System.Collections.Generic;
using System.Linq;
using Dragon.Interfaces;

namespace Dragon.Common.Objects.Tree
{
    public class TreeBuilder
    {
        public static IEnumerable<ITreeNode<TID, TData>> Build<TID, TData, TLink>(
            IEnumerable<TLink> links,
            Func<TLink, TID> parentSelector,
            Func<TLink, TID> childSelector,
            Func<TID, TData> dataSelector)
        {
            var nodes = new List<TreeNode<TID, TData>>();

            var sourceLinks = links.Select(x => new Link<TID>(parentSelector(x), childSelector(x)));

            foreach (var link in sourceLinks)
            {
                var parent = nodes.FirstOrDefault(x => x.Node.Equals(link.Parent));
                var child = nodes.FirstOrDefault(x => x.Node.Equals(link.Child));

                if (parent == null)
                {
                    parent = new TreeNode<TID, TData>();
                    parent.Node = link.Parent;
                    parent.Data = dataSelector(parent.Node);
                    nodes.Add(parent);
                }

                if (child == null)
                {
                    child = new TreeNode<TID, TData>();
                    child.Node = link.Child;
                    child.Data = dataSelector(child.Node);
                    nodes.Add(child);
                }

                foreach (var node in nodes)
                {
                    // nodes cannot be subnodes of themselves!
                    if (child.HasChildInTree(parent.Node))
                        throw new InvalidOperationException("Cannot add node to subtree of itself");
                }

                parent.Children.Add(child);
            }

            // we only want absolute parents in the top node set so remove any that are in the top
            foreach (var child in nodes.ToArray())
            {
                if (nodes.Any(x => x != child && x.HasChildInTree(child.Node)))
                {
                    nodes.Remove(child);
                }
            }

            return nodes;
        }

        private class Link<T>
        {
            public Link(T parent, T child)
            {
                Parent = parent;
                Child = child;
            }
            public T Parent;
            public T Child;

        }
    }
}
