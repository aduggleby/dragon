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
    public class CPRDeleteProjection<T, TOriginal, TArchive> : CPRSingleton,
        ICPRDatabaseProjection<T>
        where T : CPRCommand
        where TOriginal : class
        where TArchive : class, TOriginal, ICPRArchiveTable

    {
        protected static IMapper s_mapper;

        protected static Func<T, Guid> s_srcKey;
        protected static Func<TOriginal, Guid> s_origKey;

        protected IRepository<TOriginal> m_originalRepository;
        protected IRepository<TArchive> m_archiveRepository;

       
        static CPRDeleteProjection()
        {
            s_mapper = new MapperConfiguration(cfg => cfg.CreateMap<TOriginal, TArchive>()).CreateMapper();

            var srcKeyProps = typeof(T).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (srcKeyProps.Count() != 1)
                throw new Exception("CPRAutoProjection requires one property with Key attribute which TSrc does not have.");


            var originalKeyProps = typeof(TOriginal).GetProperties().Where(x => x.GetCustomAttributes(true).Any(a => a is KeyAttribute));
            if (originalKeyProps.Count() != 1)
                throw new Exception("CPRAutoProjection requires one property with Key attribute which TDest does not have.");


            var srcKeyProperty = srcKeyProps.FirstOrDefault();
            var originalKeyProperty = originalKeyProps.FirstOrDefault();

            if (!(srcKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("CPRAutoProjection requires a Guid key property which T does not have.");

            if (!(originalKeyProperty.PropertyType == typeof(Guid)))
                throw new Exception("CPRAutoProjection requires a Guid key property which TOriginal does not have.");


            s_srcKey = (o) => (Guid)srcKeyProperty.GetValue(o, null);
            s_origKey = (o) => (Guid)originalKeyProperty.GetValue(o, null);
        }

        public CPRDeleteProjection(
            IRepository<TOriginal> originalRepository,
            IRepository<TArchive> archiveRepository
            )
            : this()
        {
            m_originalRepository = originalRepository;
            m_archiveRepository = archiveRepository;

        }

        public CPRDeleteProjection()
        {

        }
        
        public virtual void Project(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, T cmd, ILoggerFactory loggerFactory, IConfiguration config)
        {
            if (m_originalRepository!= null)
            {
                throw new Exception("You must either pass in both repositories to use ICPRDatabaseProject.Project with an IDbConnectionContextFactory.");
            }
            if (m_archiveRepository != null)
            {
                throw new Exception("You must either pass in both repositories to use ICPRDatabaseProject.Project with an IDbConnectionContextFactory.");
            }
         
            var originalRepository = new Repository<TOriginal>(connectionCtxFactory, loggerFactory, config);
            var archiveRepository= new Repository<TArchive>(connectionCtxFactory, loggerFactory, config);

            ProjectToTable(originalRepository, archiveRepository, connectionCtxFactory, ctx, cmd, loggerFactory, config);
        }

        public virtual void Project(ICPRContext ctx, T cmd)
        {
            if (m_originalRepository == null || m_archiveRepository == null)
            {
                throw new Exception("You must pass repositories into the constructor to use ICPRDatabaseProject.Project(ICPRContext ctx, T cmd).");
            }

            ProjectToTable(m_originalRepository, m_archiveRepository, null, ctx, cmd, null, null);
        }

        protected void ProjectToTable(
            IRepository<TOriginal> originalRepository, 
            IRepository<TArchive> archiveRepository, 
            IDbConnectionContextFactory connectionCtxFactory, 
            ICPRContext ctx, 
            T cmd, 
            ILoggerFactory loggerFactory, 
            IConfiguration config)
        {
            var id = s_srcKey(cmd);
            var orig = originalRepository.Get(id);
            var arch = Activator.CreateInstance<TArchive>();

            if (orig != null)
            {
                s_mapper.Map(orig, arch);

                arch.LUID = Guid.NewGuid();
                arch.DeletedByUserID = ctx.UserID;
                arch.DeletedOn = DateTime.UtcNow;

                archiveRepository.Insert(arch);
                originalRepository.Delete(orig);
            }
        }

        public virtual void Unproject(ICPRContext ctx, T cmd)
        {
            // NOP, can only work in transactions!
        }
    }
}
