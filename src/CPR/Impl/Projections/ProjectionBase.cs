using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Interfaces;
using Dragon.Context;
using Dragon.Interfaces;
using StructureMap;

namespace Dragon.CPR.Impl.Projections
{
    public abstract class ProjectionBase<T> : IProjection<T>
        where T : class
    {
        public IPermissionStore PermissionStore { get; set; }
        public IProfileStore ProfileStore { get; set; }
        public IContainer Container { get; set; }
        public DragonContext Ctx { get; set; }
        
        public abstract void Project(T t);

    }
}
