using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Common.Objects.Tree
{
    public class TreeNode<T, TData> : ITreeNode<T, TData>
    {
        public TreeNode()
        {
            Children = new List<ITreeNode<T, TData>>();
        }

        public T Node { get; set; }

        public List<ITreeNode<T, TData>> Children { get; set; }

        public TData Data { get; set; }

        public bool HasChildInTree(T key)
        {
            return Node.Equals(key) || Children.Any(x=>x.HasChildInTree(key));
        }

        public ITreeNode<T, TData> GetChildInTree(T key)
        {
            if (Node.Equals(key)) return this;

            foreach (var child in Children)
            {
                var n = child.GetChildInTree(key);

                if (n!=null) return n;
            }

            return null;
        }
    }
}
