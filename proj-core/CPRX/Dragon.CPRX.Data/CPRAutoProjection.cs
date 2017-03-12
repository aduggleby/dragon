using System;
using System.Collections.Generic;
using System.Text;
using Dragon.Data.Interfaces;
using AutoMapper;
using System.Reflection;
using System.Linq;
using Dragon.Data.Attributes;
using Dragon.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Dragon.CPRX
{
    public class CPRAutoProjection<T, TDest> : ICPRDatabaseProjection<T>
        where T : CPRCommand
        where TDest : class
    {
        protected static IMapper s_mapper;

        static CPRAutoProjection()
        {
            s_mapper = new MapperConfiguration(cfg => cfg.CreateMap<T, TDest>()).CreateMapper();
        }

        private IRepository<TDest> m_destinationRepository;

        private readonly Func<T, Guid> m_srcKey;
        private readonly Func<TDest, Guid> m_destKey;
        private readonly Action<TDest, Guid> m_setDestKey;
     
        public CPRAutoProjection(IRepository<TDest> destinationRepository)
            : this()
        {
            m_destinationRepository = destinationRepository;
        }

        public CPRAutoProjection()
        {
            var srcKeyProps = typeof(T).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (srcKeyProps.Count() != 1)
                throw new Exception("CPRAutoProjection requires one property with Key attribute which TSrc does not have.");


            var destKeyProps = typeof(TDest).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (destKeyProps.Count() != 1)
                throw new Exception("CPRAutoProjection requires one property with Key attribute which TDest does not have.");

            var srcKeyProperty = srcKeyProps.FirstOrDefault();
            var destKeyProperty = destKeyProps.FirstOrDefault();

            if (!(srcKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("CPRAutoProjection requires a Guid key property which TSrc does not have.");

            if (!(destKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("CPRAutoProjection requires a Guid key property which TDest does not have.");

            m_srcKey = (o) => (Guid)srcKeyProperty.GetValue(o, null);
            m_destKey = (o) => (Guid)destKeyProperty.GetValue(o, null);
            m_setDestKey = (o, k) => destKeyProperty.SetValue(o, k, null);

        }


        public void Project(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, T cmd, ILoggerFactory loggerFactory, IConfiguration config)
        {
            if (m_destinationRepository != null)
            {
                throw new Exception("You must either pass in a repository to use ICPRDatabaseProject.Project with an IDbConnectionContextFactory.");
            }

            m_destinationRepository = new Repository<TDest>(connectionCtxFactory, loggerFactory, config);

            ProjectToTable(ctx, cmd);
        }

        public void Project(ICPRContext ctx, T cmd)
        {
            if (m_destinationRepository == null)
            {
                throw new Exception("You must either pass in a repository to use ICPRDatabaseProject.Project with an IDbConnectionContextFactory.");
            }

            ProjectToTable(ctx, cmd);
        }

        protected void ProjectToTable(ICPRContext ctx, T cmd)
        {
            var id = m_srcKey(cmd);
            var dest = m_destinationRepository.Get(id);

            var newObject = (dest == null);

            if (newObject)
            {
                dest = Activator.CreateInstance<TDest>();
            }

            s_mapper.Map(cmd, dest);

            Intercept(ctx, newObject, dest);

            if (newObject)
            {
                m_destinationRepository.Insert(dest);
            }
            else
            {
                m_destinationRepository.Update(dest);
            }
        }

        protected void Intercept(ICPRContext ctx, bool created, TDest dest)
        {
            if (dest is ICPRTable)
            {

                var table = (ICPRTable)dest;

                if (created)
                {
                    table.CreatedByUserID = ctx.UserID;
                    table.CreatedOn = DateTime.UtcNow;
                }

                table.ModifiedByUserID = ctx.UserID;
                table.ModifiedOn = DateTime.UtcNow;
            }
        }

        public void Unproject(ICPRContext ctx, T cmd)
        {
            throw new NotImplementedException("Use with CPRTranscationableExecutor in order to be able to unproject on errors.");
        }
    }
}
