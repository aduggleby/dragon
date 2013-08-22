using System;

namespace Dragon.CPR.Interfaces
{
    public interface IProjection<T>
    {
        void Project(T t);
    }
}
