using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Tests.PermissionStore.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class has_one_right
    {
        protected InMemoryPermissionStore store = new InMemoryPermissionStore();

        [TestMethod]
        public void and_hasright_returns_true()
        {
            var g = Guid.NewGuid();
            var s = Guid.NewGuid();
            
            store.AddRight(g, s, "test", false);

            store.HasRight(g, s, "test").Should().BeTrue();

        }

        [TestMethod]
        public void and_adding_another_right_does_not_cause_old_right_to_be_removed()
        {
            var g = Guid.NewGuid();
            var s = Guid.NewGuid();
            var s2 = Guid.NewGuid();

            store.AddRight(g, s, "test", false);
            store.AddRight(g, s2, "test", false);
            var sb = store.DebugOutputTree().ToString();
            store.HasRight(g, s, "test").Should().BeTrue();
            store.HasRight(g, s2, "test").Should().BeTrue();

        }
    }
}
