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
    public class has_nodes : Base
    {
        [TestInitialize]
        public void LocalSetup()
        {
            base.Setup();

            var s = store.DebugOutputTree().ToString();
        }

        [TestMethod]
        public void and_direct_children_are_returned_correctly()
        {
            store.IsDirectChildNodeOf(n1, n1_1).Should().BeTrue();
            store.IsDirectChildNodeOf(n1, n1_2).Should().BeTrue();
            store.IsDirectChildNodeOf(n1_2, n1_2_1).Should().BeTrue();
            store.IsDirectChildNodeOf(n1_2, n1_2_2).Should().BeTrue();
            store.IsDirectChildNodeOf(n1_2, n1_2_3).Should().BeTrue();
            store.IsDirectChildNodeOf(n1_2_3, n1_2).Should().BeFalse();
            store.IsDirectChildNodeOf(n1_2, n1).Should().BeFalse();
            store.IsDirectChildNodeOf(Guid.NewGuid(), n1).Should().BeFalse();

        }

        [TestMethod]
        public void and_inherited_children_are_returned_correctly()
        {
            store.IsChildNodeOf(n1, n1_2_1).Should().BeTrue();
            store.IsChildNodeOf(n1, special).Should().BeTrue();

            store.IsChildNodeOf(n1_2, n1).Should().BeFalse();
            store.IsChildNodeOf(Guid.NewGuid(), n1).Should().BeFalse();


        }

        [TestMethod]
        public void and_getrights_gives_no_rights_for_removed_child()
        {
            store.RemoveNode(n1, n1_2);
            store.HasRight(n1_2, s2, WRITE).Should().BeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(NodeDoesNotExistException))]
        public void and_node_that_does_not_exist_throws_exception()
        {
            store.RemoveNode(Guid.NewGuid(), Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(NodeDoesNotExistException))]
        public void and_getrights_throws_exception_for_n1_1_1()
        {
            store.RemoveNode(n1_1, n1_1_1);
            store.GetRightsOnNodeWithInherited(n1_1_1);
        }

        [TestMethod]
        public void and_getnodes_returns_correct_items()
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
