using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using Dragon.Data.Attributes;
using Dragon.Data.Interfaces;
using StructureMap;
using StructureMap.Attributes;
using IContainer = StructureMap.IContainer;

namespace Dragon.CPR.Impl.Projections
{
    public abstract class SingleProjectionBase<TSrc, TDest> : TargetProjectionBase<TSrc, TDest>
        where TSrc : class
        where TDest : class
    {
        private readonly Func<TSrc, Guid> m_srcKey;
        private readonly Func<TDest, Guid> m_destKey;
        private readonly Action<TDest, Guid> m_setDestKey;

        protected static IMapper s_mapper;

        static SingleProjectionBase()
        {
            s_mapper = new MapperConfiguration(cfg => cfg.CreateMap<TSrc, TDest>()).CreateMapper();
        }

        public SingleProjectionBase()
            : base()
        {
  

            var srcKeyProps = typeof(TSrc).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (srcKeyProps.Count() != 1)
                throw new Exception("SingleProjectionBase requires one property with Key attribute which TSrc does not have.");


            var destKeyProps = typeof(TDest).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (destKeyProps.Count() != 1)
                throw new Exception("SingleProjectionBase requires one property with Key attribute which TDest does not have.");

            var srcKeyProperty = srcKeyProps.FirstOrDefault();
            var destKeyProperty = destKeyProps.FirstOrDefault();

            if (!(srcKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("SingleProjectionBase requires a Guid key property which TSrc does not have.");

            if (!(destKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("SingleProjectionBase requires a Guid key property which TDest does not have.");

            m_srcKey = (o) => (Guid)srcKeyProperty.GetValue(o, null);
            m_destKey = (o) => (Guid)destKeyProperty.GetValue(o, null);
            m_setDestKey = (o, k) => destKeyProperty.SetValue(o, k, null);


        }

        protected virtual Func<TSrc, Guid> SrcKey
        {
            get { return m_srcKey; }
        }

        protected virtual Func<TDest, Guid> DestKey
        {
            get { return m_destKey; }
        }

        protected virtual Action<TDest, Guid> SetDestKey
        {
            get { return m_setDestKey; }
        }

        public override void Project(TSrc src)
        {
            var id = SrcKey(src);
            var dest = FetchDest(id);

            var newObject = (dest == null);

            if (newObject)
            {
                dest = Activator.CreateInstance<TDest>();
            }

            Map(src, dest);

            if (newObject)
            {
                Insert(dest);
            }
            else
            {
                Save(dest);
            }
        }


        protected virtual TSrc FetchSrc(Guid id)
        {
            return RepositorySource.Get(id);
        }

        protected virtual TDest FetchDest(Guid id)
        {
            return RepositoryDestination.Get(id);
        }

        protected virtual void Map(TSrc src, TDest dest)
        {
            s_mapper.Map(src, dest);

            base.Intercept(dest);
        }


        protected virtual void Insert(TDest t)
        {
            RepositoryDestination.Insert(t);
        }

        protected virtual void Save(TDest t)
        {
            RepositoryDestination.Update(t);
        }
    }
}
