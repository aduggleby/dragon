using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Dragon.Data.Interfaces;
using Dragon.Data.Attributes;
using Dragon.Data.Repositories;
using Dragon.CPRX.Data;

namespace Dragon.CPRX.Test
{
    [TestClass]
    public class OverrideTest
    {
        private CPRTransactionableExecutor m_executor;

        private DefaultDbConnectionContextFactory m_connectionFactory;
        private ILoggerFactory m_loggerFactory;
        private IConfiguration m_config;
        private RepositorySetup m_setup;
        public OverrideTest()
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
        public void InterceptCanBeOverridden()
        {
            var called = false;
            OverriddenActionProjection.InterceptCalled = () => called = true;
            var instance = OverriddenActionProjection.Instance<OverriddenActionProjection>();

            var r = m_executor.Execute(new OverridenDataTestCommand());

            Assert.IsTrue(called, "Intercept was not called.");
        }


    }

    public class OverridenDataTestCommand : CPRCommand<OverridenDataTestCommand>,
        ICPRSecurityValidator<OverridenDataTestCommand>
    {
        public OverridenDataTestCommand()
        {
        }

        [Key]
        public Guid TableID { get; set; }

        public override IEnumerable<ICPRSecurityValidator<OverridenDataTestCommand>> SecurityValidators =>
            new ICPRSecurityValidator<OverridenDataTestCommand>[] { this };

        public override IEnumerable<ICPRProjectionBase<OverridenDataTestCommand>> Projections
        {

            get
            {
                yield return (ICPRProjectionBase<OverridenDataTestCommand>)OverriddenActionProjection.Instance< OverriddenActionProjection>();
            }
        }

        public IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, OverridenDataTestCommand cmd)
        {
            yield break;
        }
    }

    public class OverriddenActionProjection : CPRAutoProjection<OverridenDataTestCommand, DataTable>
    {
        public static Action InterceptCalled;

        protected override void Intercept(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, bool created, DataTable dest, ILoggerFactory loggerFactory, IConfiguration config)
        {
            InterceptCalled();

        }
    }
}
