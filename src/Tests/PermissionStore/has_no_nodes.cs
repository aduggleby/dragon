using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Context.Exceptions;
using Dragon.Tests.PermissionStore.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class has_no_nodes
    {
        protected InMemoryPermissionStore store = new InMemoryPermissionStore();

        [TestMethod]
        [ExpectedException(typeof(NodeDoesNotExistException))]
        public void and_getnodes_throw_exception()
        {
            store.GetRightsOnNodeWithInherited(Guid.NewGuid());
        }
    }
}
