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
    public class has_multiple : Base
    {
        private Guid n1 = Guid.NewGuid();
        private Guid n2 = Guid.NewGuid();
        private Guid n3 = Guid.NewGuid();
        private Guid n4 = Guid.NewGuid();

        private string r1 = Guid.NewGuid().ToString();
        private string r2 = Guid.NewGuid().ToString();
        private string r3 = Guid.NewGuid().ToString();


        [TestInitialize]
        public void LocalSetup()
        {
            store.AddRight(n1, s1, r1, true);
            store.AddRight(n2, s2, r2, true);
            store.AddRight(n3, s3, r2, false);
            store.AddRight(n3, s3, r3, false);

            var o1 = store.DebugOutputTree().ToString();

            store.AddRight(n1, s3, r1, false);
            store.AddRight(n2, s2, r2, false);
            store.AddRight(n3, s1, r2, true);
            store.AddRight(n3, s1, r3, false);

            var o2 = store.DebugOutputTree().ToString();
        }

        [TestMethod]
        public void and_nodes_are_not_duplicated()
        {
            store.Tree.Select(x => x.GetChildInTree(n1)).Count(x=>x!=null).Should().Be(1);
            store.Tree.Select(x => x.GetChildInTree(n2)).Count(x=>x!=null).Should().Be(1);
            store.Tree.Select(x => x.GetChildInTree(n3)).Count(x => x != null).Should().Be(1);
            store.Tree.Select(x => x.GetChildInTree(n4)).Count(x => x != null).Should().Be(0);
        }
    }
}
