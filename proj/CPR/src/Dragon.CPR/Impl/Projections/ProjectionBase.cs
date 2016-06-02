using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Interfaces;
using Dragon.Data.Interfaces;
using StructureMap;

namespace Dragon.CPR.Impl.Projections
{
    public abstract class ProjectionBase<T> : IProjection<T>
        where T : class
    {
        public IContainer Container { get; set; }
        
        public abstract void Project(T t);

    }
}
