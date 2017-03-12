using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Dragon.Data.Attributes;
using System.Linq;
using Dragon.Data.Repositories;
using Dragon.CPRX.Data;

namespace Dragon.CPRX.Test
{
    [TestClass]
    public class DataTest
    {
        private CPRTransactionableExecutor m_executor;

        private DefaultDbConnectionContextFactory m_connectionFactory;
        private ILoggerFactory m_loggerFactory;
        private IConfiguration m_config;
        private RepositorySetup m_setup;
        public DataTest()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "Dragon:Data:ConnectionString",$@"Server=localhost;Database=DRAGON_CPRX_TEST;Trusted_Connection=True;" }
                });


            m_config = builder.Build();

            m_loggerFactory = new LoggerFactory();

            m_connectionFactory = new DefaultDbConnectionContextFactory(m_config, m_loggerFactory);

            m_setup = new RepositorySetup(m_connectionFactory, m_config, m_loggerFactory);

            var ctxFetcher = new TestContextFetcher(new TestContext());

            m_executor = new CPRTransactionableExecutor(new TestStore(), ctxFetcher, m_config, m_loggerFactory);

        }

        [TestInitialize]
        public void Setup()
        {
            m_setup.EnsureTableExists<DataTable>();
        }

        [TestCleanup]
        public void Clean()
        {
            m_setup.DropTableIfExists<DataTable>();
        }

        [TestMethod]
        public void Table_has_been_created()
        {
            var rep = new Repository<DataTable>(m_connectionFactory, m_loggerFactory, m_config);

            var all = rep.GetAll();
        }

        [TestMethod]
        public void Table_is_projected()
        {
            var id = Guid.NewGuid();
            var r = m_executor.Execute(new DataTestCommand() { TableID = id });
            
            var rep = new Repository<DataTable>(m_connectionFactory, m_loggerFactory, m_config);
            var t = rep.Get(id);

            Assert.IsNotNull(t);
        }

    }

    public class DataTestCommand : CPRCommand<DataTestCommand>
    {
        public DataTestCommand()
        {
        }

        [Key]
        public Guid TableID { get; set; }


        public override IEnumerable<ICPRValidator<DataTestCommand>> Validators => new ICPRValidator<DataTestCommand>[] { };

        public override IEnumerable<ICPRInterceptor<DataTestCommand>> Interceptors => base.Interceptors;

        public override IEnumerable<ICPRSecurityValidator<DataTestCommand>> SecurityValidators => new ICPRSecurityValidator<DataTestCommand>[] { new DataPassSecurityValidator() };

        public override IEnumerable<ICPRProjection<DataTestCommand>> Projections
        {

            get
            {
                yield return new CPRAutoProjection<DataTestCommand, DataTable>();
            }
        }
    }



    public class DataPassSecurityValidator : ICPRSecurityValidator<DataTestCommand>
    {
        public IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, DataTestCommand cmd)
        {
            yield break;
        }
    }

    public class DataTable : ICPRTable
    {
        [Key]
        public Guid TableID { get; set; }

        public Guid CreatedByUserID { get; set; }
        public Guid ModifiedByUserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
