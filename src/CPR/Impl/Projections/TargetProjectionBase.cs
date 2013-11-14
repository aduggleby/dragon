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
    public abstract class TargetProjectionBase<TSrc, TDest> : InterceptingProjectionBase<TSrc, TDest>
        where TSrc : class
        where TDest : class
    {
        public IRepository<TSrc> RepositorySource { get; set; }
        public IRepository<TDest> RepositoryDestination { get; set; }
        public IPermissionStore PermissionStore { get; set; }
        public DragonContext Ctx { get; set; }


        public IContainer Container { get; set; }

     

     
    }
}
