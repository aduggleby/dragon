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
                    Spec = READ,
                    Inherit = true,
                    Inherited = false,
                },
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = s2.ToString(),
                    Spec = READ,
                    Inherit = false,
                    Inherited = false,
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
                    Spec = READ,
                    Inherit = true,
                    Inherited = true,
                },
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = s2.ToString(),
                    Spec = WRITE,
                    Inherit = false,
                    Inherited = false,
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

        [TestMethod]
        public void and_permission_info_extractor_returns_correct_info_for_subjects()
        {
            store.AddRight(n1, s2, MANAGE, true);

            var expected = new List<IPermissionInfo>
            {
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = n1.ToString(),
                    Spec = READ,
                    Inherit = false,
                    Inherited = false,
                },
                new Context.Permissions.PermissionInfo
                {
                    DisplayName = n1_2.ToString(),
                    Spec = WRITE,
                    Inherit = false,
                    Inherited = false,
                },
            };

            var guids = new Guid[] { n1, n1_1, n1_2, n1_1_1, special, n1_2, n1_2_1, n1_2_2, n1_2_3 };
            foreach (var g in guids)
            {
                expected.Add(new Context.Permissions.PermissionInfo
                {
                    DisplayName = g.ToString(),
                    Spec = MANAGE,
                    Inherit = true,
                    Inherited = g != n1,
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
        }
    }
}
