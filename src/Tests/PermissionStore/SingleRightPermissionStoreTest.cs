using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Tests.PermissionStore.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class SingleRightPermissionStoreTest
    {
        protected InMemoryPermissionStore store = new InMemoryPermissionStore();

        [TestMethod]
        public void SingleRight_ReturnsTrue()
        {
            var g = Guid.NewGuid();
            var s = Guid.NewGuid();
            
            store.AddRight(g, s, "test", false);

            store.HasRight(g, s, "test").Should().BeTrue();

        }
    }
}
