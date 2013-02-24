using System.Collections.Generic;

namespace Dragon.Interfaces
{
    public interface ITreeNode<T, TData>
    {
        T Node { get; set; }
        List<ITreeNode<T, TData>> Children { get; set; }
        TData Data { get; set; }
        bool HasChildInTree(T key);
    }
}