using System;
using System.Collections.Generic;
using System.Linq;
using Dragon.Context.Permissions;
using Dragon.Interfaces;
using Dragon.Tests.PermissionStore.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Tests.PermissionStore.PermissionInfo
{
    [TestClass]
    public class has_rights : Base
    {
        [TestInitialize]
        public void LocalSetup()
        {
            Setup();
        }

        [TestMethod]
        public void and_permission_info_extractor_returns_correct_info_for_root_node()
        {
            var expected = new List<IPermissionInfo>
            {
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = s1.ToString(),
                    Spec = GetPermissionRightString(n1, s1, READ),
                    Inherit = true,
                    InheritedFrom = null,
                },
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = s2.ToString(),
                    Spec = GetPermissionRightString(n1, s2, READ),
                    Inherit = false,
                    InheritedFrom = null,
                },
            };
            var nameResolver = new DefaultNameResolver();
            var infoExtractor = new PermissionInfoExtractor(store, nameResolver);
            var actual = infoExtractor.GetPermissionInfoForNode(n1).ToList();

            actual.Should().NotBeNull();
            actual.Count().Should().Be(expected.Count);
            foreach (var ex in expected)
            {
                actual.Should().Contain(ex);
            }
        }

        [TestMethod]
        public void and_permission_info_extractor_returns_correct_info_for_child_node()
        {
            var expected = new List<IPermissionInfo>
            {
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = s1.ToString(),
                    Spec =  GetPermissionRightString(n1_2, s1, READ),
                    Inherit = true,
                    InheritedFrom = n1.ToString(),

                },
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = s2.ToString(),
                    Spec = GetPermissionRightString(n1_2, s2, WRITE),
                    Inherit = false,
                    InheritedFrom = null,
                },
            };
            var nameResolver = new DefaultNameResolver("");
            var infoExtractor = new PermissionInfoExtractor(store, nameResolver);
            var actual = infoExtractor.GetPermissionInfoForNode(n1_2).ToList();

            actual.Should().NotBeNull();
            actual.Count().Should().Be(expected.Count);
            foreach (var ex in expected)
            {
                actual.Should().Contain(ex);
            }
        }

        private string GetPermissionRightString(Guid node, Guid subject, string spec)
        {
            foreach (var permissionRight in store.GetRightsOnNodeWithInherited(node).Where(
                permissionRight => permissionRight.Spec == spec && permissionRight.SubjectID == subject))
            {
                return spec + " (" + permissionRight.LID.ToString().Substring(0,4) + ")";
            }
            throw new Exception("Right not found on node.");
        }

        [TestMethod]
        public void and_permission_info_extractor_returns_correct_info_for_subjects()
        {
            store.AddRight(n1, s2, MANAGE, true);

            var expected = new List<IPermissionInfo>
            {
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = n1.ToString(),
                    Spec = GetPermissionRightString(n1, s2, READ),
                    Inherit = false,
                    InheritedFrom = null,
                },
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = n1_2.ToString(),
                    Spec = GetPermissionRightString(n1_2, s2, WRITE),
                    Inherit = false,
                    InheritedFrom = null,
                },
            };

            var guids = new[] { n1, n1_1, n1_2, n1_1_1, special, n1_2_1, n1_2_2, n1_2_3 };
            foreach (var g in guids)
            {
                expected.Add(new Context.Permissions.PermissionInfo
                {
                    DisplayName = g.ToString(),
                    Spec = GetPermissionRightString(g, s2, MANAGE),
                    Inherit = true,
                    InheritedFrom = g != n1 ? n1.ToString() : null,
                });
            }

            var nameResolver = new DefaultNameResolver("");
            var infoExtractor = new PermissionInfoExtractor(store, nameResolver);
            var actual = infoExtractor.GetPermissionInfoForSubject(s2).ToList();

            actual.Should().NotBeNull();
            actual.Count().Should().Be(expected.Count);
            foreach (var ex in expected)
            {
                actual.Should().Contain(ex);
            }
            foreach (var ac in actual)
            {
                expected.Should().Contain(ac);
            }
        }
    }
}
