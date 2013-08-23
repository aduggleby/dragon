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
    public class has_rights : Base
    {
        [TestInitialize]
        public void LocalSetup()
        {
            base.Setup();

            var s = store.DebugOutputTree().ToString();
        }

        [TestMethod]
        public void and_read_rights_are_inherited()
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

            store.GetRightsOnNodeWithInherited(special).Count().Should().Be(2);

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

        [TestMethod]
        public void and_isInherited_returns_correct_result()
        {
            store.IsRightInherited(n1, s1, READ).Should().Be(false);
            store.IsRightInherited(n1_1, s1, READ).Should().Be(true);
            store.IsRightInherited(n1_1_1, s1, READ).Should().Be(true);
            store.IsRightInherited(special, s1, READ).Should().Be(true);
            store.IsRightInherited(n1, s2, READ).Should().Be(false);
            store.IsRightInherited(n1_1, s2, READ).Should().Be(false);
            store.IsRightInherited(n1_1_1, s2, READ).Should().Be(false);
            store.IsRightInherited(special, s2, READ).Should().Be(false);
        }
    }
}
