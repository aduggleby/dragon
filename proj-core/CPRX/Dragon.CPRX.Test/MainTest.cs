using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Dragon.CPRX.Test
{
    [TestClass]
    public class MainTest
    {
        private CPRExecutor m_executor;

        public MainTest()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {

                });

            var config = builder.Build();

            var loggerFactory = new LoggerFactory();

            var ctxFetcher = new TestContextFetcher(new TestContext());

            m_executor = new CPRExecutor(new TestStore(), ctxFetcher, config, loggerFactory);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PassSecurity_Passes()
        {
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { new ThrowsExceptionValidator() },
                new ICPRProjection<TestCommand>[] { }));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BasePassSecurity_Passes()
        {
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<CPRCommand>[] { new PassBaseSecurityValidator() },
                new ICPRValidator<TestCommand>[] { new ThrowsExceptionValidator() },
                new ICPRProjection<TestCommand>[] { }));
        }

        [TestMethod]
        public void FailSecurity_Fails()
        {
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new FailSecurityValidator() },
                new ICPRValidator<TestCommand>[] { new ThrowsExceptionValidator() },
                new ICPRProjection<TestCommand>[] { }));

            Assert.AreEqual(false, r.Success);
        }

        [TestMethod]
        public void FailValidator_Fails()
        {
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { new FailValidator() },
                new ICPRProjection<TestCommand>[] { new ThrowsExceptionProjection() }));

            Assert.AreEqual(false, r.Success);
        }

        [TestMethod]
        public void PassValidator_Passes()
        {
            var projected = false;

            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { new PassValidator() },
                new ICPRProjection<TestCommand>[] { new ActionProjection(() => projected = true, () => { }) }));

            Assert.AreEqual(true, r.Success);
            Assert.AreEqual(true, projected);
        }

        [TestMethod]
        public void MultiplePassValidator_Passes()
        {
            var projected = false;
            var validators = 0;
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { new PassValidator(() => validators++), new PassValidator(() => validators++) },
                new ICPRProjection<TestCommand>[] { new ActionProjection(() => projected = true, () => { }) }));

            Assert.AreEqual(true, r.Success);
            Assert.AreEqual(true, projected);
            Assert.AreEqual(2, validators);
        }


        [TestMethod]
        public void MultiplePassFailPassValidator_CallsAllValidators()
        {
            var projected = false;
            var validators = 0;
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] {
                    new PassValidator(() => validators++),
                    new FailValidator(),
                    new PassValidator(() => validators++) },
                new ICPRProjection<TestCommand>[] { new ActionProjection(() => projected = true, () => { }) }));

            Assert.AreEqual(false, r.Success);
            Assert.AreEqual(false, projected);
            Assert.AreEqual(2, validators);
        }

        [TestMethod]
        public void MultiplePassValidator_CallsAllValidatorsInOrder()
        {
            var projected = false;
            var validators = 0;
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] {
                    new PassValidator(() => validators=1),
                    new PassValidator(() => validators=2),
                    new PassValidator(() => validators=3) },
                new ICPRProjection<TestCommand>[] { new ActionProjection(() => projected = true, () => { }) }));

            Assert.AreEqual(true, r.Success);
            Assert.AreEqual(true, projected);
            Assert.AreEqual(3, validators);
        }

        [TestMethod]
        public void Projection_OnlyProjectCalledOnSuccess()
        {
            var projected = 0;
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { },
                new ICPRProjection<TestCommand>[] {
                    new ActionProjection(() => projected ++, () => projected--) }));

            Assert.AreEqual(true, r.Success);
            Assert.AreEqual(1, projected);
        }

        [TestMethod]
        public void Projection_UnprojectCalledOnException()
        {
            var projected = 0;
            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { },
                new ICPRProjection<TestCommand>[] {
                    new ActionProjection(() => projected ++, () => projected--),
                    new ActionProjection(() => projected ++, () => projected--),
                    new ActionProjection(() => {
                        projected ++;
                        throw new Exception("Boom");
                        }, () => projected--),
                    new ActionProjection(() => projected ++, () => projected--)

                }));

            Assert.AreEqual(false, r.Success);
            Assert.AreEqual(0, projected);
        }


        [TestMethod]
        public void Projection_UnprojectCalledInReverseOrder()
        {
            var projected = 0;

            var r = m_executor.Execute(new TestCommand(
                new ICPRSecurityValidator<TestCommand>[] { new PassSecurityValidator() },
                new ICPRValidator<TestCommand>[] { },
                new ICPRProjection<TestCommand>[] {
                    new ActionProjection(() => { }, () => projected=1),
                    new ActionProjection(() => { }, () => projected=2),
                    new ActionProjection(() => throw new Exception("Boom"), () => projected=3),
                    new ActionProjection(() =>{ }, () => projected=4)

                }));

            Assert.AreEqual(false, r.Success);
            Assert.AreEqual(1, projected);
        }
    }

    public class TestStore : ICPRStore
    {
        public void Persist(CPRCommand cmd, Guid executingUserID, DateTime utcBeganProjections)
        {
            // NOP
        }
    }

    public class TestContextFetcher : ICPRContextFetcher
    {
        private ICPRContext m_ctx;
        public TestContextFetcher(ICPRContext ctx)
        {
            m_ctx = ctx;
        }
        public ICPRContext GetCurrentContext()
        {
            return m_ctx;
        }
    }

    public class TestContext : ICPRContext
    {
        public Guid UserID => Guid.NewGuid();
    }

    public class TestCommand : CPRCommand<TestCommand>
    {
        private IEnumerable<ICPRSecurityValidator<TestCommand>> m_securityValidators;
        private IEnumerable<ICPRProjection<TestCommand>> m_projections;
        private IEnumerable<ICPRValidator<TestCommand>> m_validators;

        public TestCommand(
            IEnumerable<ICPRSecurityValidator<TestCommand>> securityValidators,
            IEnumerable<ICPRValidator<TestCommand>> validators,
            IEnumerable<ICPRProjection<TestCommand>> projections
            )
        {
            m_securityValidators = securityValidators;
            m_projections = projections;
            m_validators = validators;
        }

        public override IEnumerable<ICPRValidator<TestCommand>> Validators => m_validators;

        public override IEnumerable<ICPRInterceptor<TestCommand>> Interceptors => base.Interceptors;

        public override IEnumerable<ICPRSecurityValidator<TestCommand>> SecurityValidators => m_securityValidators;

        public override IEnumerable<ICPRProjection<TestCommand>> Projections => m_projections;
    }


    public class FailSecurityValidator : ICPRSecurityValidator<TestCommand>
    {
        public IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, TestCommand cmd)
        {
            yield return new CPRSecurityError() { Message = "Test" };
        }
    }

    public class PassSecurityValidator : ICPRSecurityValidator<TestCommand>
    {
        public IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, TestCommand cmd)
        {
            yield break;
        }
    }

    public class PassBaseSecurityValidator : ICPRSecurityValidator<CPRCommand>
    {
        public IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, CPRCommand cmd)
        {
            yield break;
        }
    }

    public class ThrowsExceptionValidator : ICPRValidator<TestCommand>
    {
        public IEnumerable<CPRError> Validate(ICPRContext ctx, CPRCommand cmd)
        {
            throw new Exception("Test");
        }
    }

    public class PassValidator : ICPRValidator<TestCommand>
    {
        private Action m_a;

        public PassValidator()
        {

        }

        public PassValidator(Action a)
        {
            m_a = a;
        }

        public IEnumerable<CPRError> Validate(ICPRContext ctx, CPRCommand cmd)
        {
            if (m_a != null) m_a();
            yield break;
        }
    }

    public class FailValidator : ICPRValidator<TestCommand>
    {
        public IEnumerable<CPRError> Validate(ICPRContext ctx, CPRCommand cmd)
        {
            yield return new CPRError() { Message = "Test" };
        }
    }

    public class ThrowsExceptionProjection : ICPRProjection<TestCommand>
    {
        public void Project(ICPRContext ctx, TestCommand cmd)
        {
            throw new Exception("This should not be called.");
        }

        public void Unproject(ICPRContext ctx, TestCommand cmd)
        {
            throw new Exception("This should not be called.");
        }
    }

    public class ActionProjection : ICPRProjection<TestCommand>
    {
        private Action m_project;
        private Action m_unproject;

        public ActionProjection(Action project, Action unproject)
        {
            m_project = project;
            m_unproject = unproject;
        }

        public void Project(ICPRContext ctx, TestCommand cmd)
        {
            m_project();
        }

        public void Unproject(ICPRContext ctx, TestCommand cmd)
        {
            m_unproject();
        }
    }
}
