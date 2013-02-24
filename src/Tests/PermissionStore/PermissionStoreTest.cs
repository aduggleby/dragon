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
    public class PermissionStoreTest : PermissionStoreTestsBase
    {
        [TestInitialize]
        public void LocalSetup()
        {
            base.Setup();

            var s = store.DebugOutputTree().ToString();
        }

        [TestMethod]
        public void ReadRights_were_properly_inherited()
        {
            foreach (var node in store.NodeListWithInheritedRights())
            {
                store.HasRight(node.Node, s1, READ).Should().BeTrue();
            }

            foreach (var node in store.NodeListWithInheritedRights())
            {
                if (!node.Node.Equals(n1))
                {
                    store.HasRight(node.Node, s2, READ).Should().BeFalse();
                }
            }
        }



        [TestMethod]
        public void Special_has_unique_right_only_once()
        {
            store.GetRightsOnNodeWithInherited(special).Count().Should().Be(2);
        }

        [TestMethod]
        public void GetNodes_returns_correct_nodes()
        {
            store.GetNodesWithRight(s2, MANAGE).Should().BeEmpty();
            var nodes = store.GetNodesWithRight(s3, MANAGE);
            nodes.Should().NotBeNull();
            nodes.Count().Should().Be(2); // n1_2_2 AND special
            nodes.Should().Contain(n1_2_2);
            nodes.Should().Contain(special);
        }
    }
}
