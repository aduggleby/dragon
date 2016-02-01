using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Context.Interfaces;
using Dragon.CPR.Interfaces;
using Dragon.Data.Interfaces;
using Dragon.Context;
using StructureMap;
using IContext = Dragon.Context.IContext;

namespace Dragon.CPR.Impl.Projections
{
    public abstract class ProjectionBase<T> : IProjection<T>
        where T : class
    {
        public IProfileStore ProfileStore { get; set; }
        public IContainer Container { get; set; }
        public IContext Ctx { get; set; }
        
        public abstract void Project(T t);

    }
}
