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
    }
}
