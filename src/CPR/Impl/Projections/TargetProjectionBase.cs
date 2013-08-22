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
    public abstract class TargetProjectionBase<TSrc, TDest> : ProjectionBase<TSrc>
        where TSrc : class
        where TDest : class
    {
        public IReadModelRepository Repository { get; set; }
        public IPermissionStore PermissionStore { get; set; }
        public DragonContext Ctx { get; set; }

        private Dictionary<Type, List<Action<TDest>>> m_interceptors;

        public IContainer Container { get; set; }

        public TargetProjectionBase()
        {   
            // TODO: Can't find a solution for this which is cleaner right now ...
            Container = ObjectFactory.Container;

            m_interceptors = new Dictionary<Type, List<Action<TDest>>>();
            var t = typeof(TDest);
            while (t != null)
            {
                var genType = typeof(IInterceptor<>).MakeGenericType(t);
                var l = new List<Action<TDest>>();
                foreach (var theinstance in Container.GetAllInstances(genType))
                {
                    var instance = theinstance;
                    Action<object> f = o =>
                                       instance.GetType()
                                               .GetMethod("Intercept")
                                               .Invoke(instance, new object[] { o });
                    l.Add(f);
                }
                m_interceptors.Add(t, l);
                t = t.BaseType;
            }
        }

        protected void Intercept(TDest dest)
        {
            foreach (var t in m_interceptors.Keys)
            {
                var list = m_interceptors[t];
                list.ForEach(x => x(dest));
            }
        }

    }
}
