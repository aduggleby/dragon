using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Context.Extensions.Groups;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragon.Context.Extensions.Groups;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class PermissionStoreGroupExtensionsTest : PermissionStoreTestsBase
    {
        protected Guid g1 = Guid.NewGuid();
        private const string RIGHT = "RTEST";
        private const string RIGHT2 = "RTEST2";

        [TestInitialize]
        public void LocalSetup()
        {
            base.Setup();

            store.AddNode(n1, g1);
            store.AddToGroup(g1, s1);
            store.AddRight(n1_1, g1, RIGHT, true);
            store.AddRight(n1_1, g1, RIGHT2, false);

            var s = store.DebugOutputTree().ToString();
        }

        [TestMethod]
        public void Rights_were_properly_inherited()
        {
            store.HasRightIncludingGroups(n1, s1, RIGHT).Should().BeFalse();
            store.HasRightIncludingGroups(n1_1, s1, RIGHT).Should().BeTrue();
            store.HasRightIncludingGroups(n1_1, s1, RIGHT2).Should().BeTrue();
            store.HasRightIncludingGroups(n1_1_1, s1, RIGHT).Should().BeTrue();
            store.HasRightIncludingGroups(n1_1_1, s1, RIGHT2).Should().BeFalse();
            store.HasRightIncludingGroups(n1_2, s1, RIGHT).Should().BeFalse();
            store.HasRightIncludingGroups(n1_2, s1, RIGHT2).Should().BeFalse();
        }

        [TestMethod]
        public void Correct_nodes_are_returned()
        {
            var set1 = store.GetNodesWithRightIncludingGroups(s1, RIGHT);
            set1.Should().HaveCount(3);
            set1.Should().Contain(n1_1);
            set1.Should().Contain(n1_1_1);
            set1.Should().Contain(special);

            var set2 = store.GetNodesWithRightIncludingGroups(s1, RIGHT2);
            set2.Should().HaveCount(1);
            set2.Should().Contain(n1_1);
        }
        

    }
}
