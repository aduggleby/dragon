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
    public class EmptyPermissionStoreTest
    {
        protected InMemoryPermissionStore store = new InMemoryPermissionStore();
        
        [TestMethod]
        public void HasRight_ReturnsFalse()
        {
            store.HasRight(Guid.Empty, Guid.Empty, "abc").Should().BeFalse();
            store.HasRight(Guid.NewGuid(), Guid.Empty, "abc").Should().BeFalse();
            store.HasRight(Guid.Empty, Guid.NewGuid(), "abc").Should().BeFalse();
            store.HasRight(Guid.NewGuid(), Guid.NewGuid(), "abc").Should().BeFalse();
        }
    }
}
