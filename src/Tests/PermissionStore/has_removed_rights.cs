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
    public class has_removed_rights : Base
    {
        [TestInitialize]
        public void LocalSetup()
        {
            base.Setup();

            var s = store.DebugOutputTree().ToString();

        }

        [TestMethod]
        public void and_read_rights_are_removed_for_s1()
        {
            store.RemoveRight(n1, s1, READ);

            foreach (var node in store.NodeListWithInheritedRights())
            {
                store.HasRight(node.Node, s1, READ).Should().BeFalse();
            }
            store.GetRightsOnNodeWithInherited(special).Count().Should().Be(1);

            store.GetNodesWithRight(s1, READ).Should().BeEmpty();
        }

        [TestMethod]
        public void and_read_rights_are_removed_for_s2()
        {
            store.RemoveRight(n1, s2, READ);

            foreach (var node in store.NodeListWithInheritedRights())
            {
                store.HasRight(node.Node, s2, READ).Should().BeFalse();
            }
            store.GetRightsOnNodeWithInherited(special).Count().Should().Be(2); // no change

            store.GetNodesWithRight(s2, READ).Should().BeEmpty();
        }

        [TestMethod]
        public void and_manage_rights_are_removed_for_s2()
        {
            store.RemoveRight(n1_2_2, s3, MANAGE);

            foreach (var node in store.NodeListWithInheritedRights())
            {
                store.HasRight(node.Node, s3, MANAGE).Should().BeFalse();
            }
            store.GetRightsOnNodeWithInherited(special).Count().Should().Be(1);

            store.GetNodesWithRight(s3, MANAGE).Should().BeEmpty();
        }

        [TestMethod]
        public void and_write_rights_are_removed_for_s2()
        {
            store.RemoveRight(n1_2, s2, WRITE);

            foreach (var node in store.NodeListWithInheritedRights())
            {
                store.HasRight(node.Node, s2, WRITE).Should().BeFalse();
            }
            store.GetRightsOnNodeWithInherited(special).Count().Should().Be(2); // no effect

            store.GetNodesWithRight(s2, WRITE).Should().BeEmpty();
        }


        [TestMethod]
        [ExpectedException(typeof(RightDoesNotExistException))]
        public void and_write_rights_are_removed_thata_were_never_added()
        {
            store.RemoveRight(n1, s2, WRITE);
        }
    }
}
