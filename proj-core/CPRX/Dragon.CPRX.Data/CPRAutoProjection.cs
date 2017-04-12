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
    public class CPRAutoProjection<T, TDest> : CPRSingleton,
        ICPRDatabaseProjection<T>
        where T : CPRCommand
        where TDest : class
    {
        protected static IMapper s_mapper;

        protected static Func<T, Guid> s_srcKey;
        protected static Func<TDest, Guid> s_destKey;
        protected static Action<TDest, Guid> s_setDestKey;

        private IRepository<TDest> m_destinationRepository;

        public CPRAutoProjection(IRepository<TDest> destinationRepository)
            : this()
        {
            m_destinationRepository = destinationRepository;
        }

        public CPRAutoProjection()
        {
        }

        static CPRAutoProjection()
        {
            s_mapper = new MapperConfiguration(cfg => cfg.CreateMap<T, TDest>()).CreateMapper();

            StringBuilder typeLog = new StringBuilder();

            typeLog.AppendLine($"TSrc is: {typeof(T)}.");
            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                typeLog.AppendLine($"  has property '{p.Name}': ");
                p.GetCustomAttributes(true).ToList().ForEach(a =>
                {
                    typeLog.AppendLine($"  - with attribute '{a.GetType().FullName}'. Is Key: {a is KeyAttribute}");
                });
            });

            typeLog.AppendLine($"TDest is: {typeof(TDest)}.");
            typeof(TDest).GetProperties().ToList().ForEach(p =>
            {
                typeLog.AppendLine($"  has property '{p.Name}' (Type: {p.PropertyType.FullName}): ");
                p.GetCustomAttributes(true).ToList().ForEach(a =>
                {
                    typeLog.AppendLine($"  - with attribute '{a.GetType().FullName}'. Is Key: {a is KeyAttribute}");
                });
            });

            var srcKeyProps = typeof(T).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (srcKeyProps.Count() != 1)
                throw new Exception("CPRAutoProjection requires one property with Key attribute which TSrc does not have." + Environment.NewLine + typeLog.ToString());


            var destKeyProps = typeof(TDest).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (destKeyProps.Count() != 1)
                throw new Exception("CPRAutoProjection requires one property with Key attribute which TDest does not have." + Environment.NewLine + typeLog.ToString());

            var srcKeyProperty = srcKeyProps.FirstOrDefault();
            var destKeyProperty = destKeyProps.FirstOrDefault();

            if (!(srcKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("CPRAutoProjection requires a Guid key property which TSrc does not have." + Environment.NewLine + typeLog.ToString());

            if (!(destKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("CPRAutoProjection requires a Guid key property which TDest does not have." + Environment.NewLine + typeLog.ToString());

            s_srcKey = (o) => (Guid)srcKeyProperty.GetValue(o, null);
            s_destKey = (o) => (Guid)destKeyProperty.GetValue(o, null);
            s_setDestKey = (o, k) => destKeyProperty.SetValue(o, k, null);

        }

        public virtual void Project(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, T cmd, ILoggerFactory loggerFactory, IConfiguration config)
        {
            var rep = new Repository<TDest>(connectionCtxFactory, loggerFactory, config);

            ProjectToTable(rep, connectionCtxFactory, ctx, cmd, loggerFactory, config);
        }

        public virtual void Project(ICPRContext ctx, T cmd)
        {
            if (m_destinationRepository == null)
            {
                throw new Exception("You must pass a repository into the constructor to use ICPRDatabaseProject.Project(ICPRContext ctx, T cmd).");
            }

            ProjectToTable(m_destinationRepository, null, ctx, cmd, null, null);
        }

        protected virtual void ProjectToTable(IRepository<TDest> repository, IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, T cmd, ILoggerFactory loggerFactory, IConfiguration config)
        {
            var id = s_srcKey(cmd);
            var dest = repository.Get(id);

            var newObject = (dest == null);

            if (newObject)
            {
                dest = Activator.CreateInstance<TDest>();
            }

            s_mapper.Map(cmd, dest);

            Intercept(connectionCtxFactory, ctx, newObject, dest, loggerFactory, config);

            if (newObject)
            {
                repository.Insert(dest);
            }
            else
            {
                repository.Update(dest);
            }
        }

        protected virtual void Intercept(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, bool created, TDest dest, ILoggerFactory loggerFactory, IConfiguration config)
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

        public virtual void Unproject(ICPRContext ctx, T cmd)
        {
            // NOP
        }
    }
}
