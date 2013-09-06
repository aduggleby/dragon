using System.Collections.Generic;
using System.Linq;
using Dragon.Context.Permissions;
using Dragon.Interfaces;
using Dragon.Tests.PermissionStore.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class permission_info_has_rights : Base
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
                new PermissionInfo
                {
                    Subject = s1.ToString(),
                    Spec = READ,
                    Inherit = true,
                    Inherited = false,
                },
                new PermissionInfo
                {
                    Subject = s2.ToString(),
                    Spec = READ,
                    Inherit = false,
                    Inherited = false,
                },
            };
            var nameResolver = new DefaultNameResolver("");
            var infoExtractor = new PermissionInfoExtractor(store, nameResolver);
            var actual = infoExtractor.GetPermissionInfo(n1).ToList();

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
                new PermissionInfo
                {
                    Subject = s1.ToString(),
                    Spec = READ,
                    Inherit = true,
                    Inherited = true,
                },
                new PermissionInfo
                {
                    Subject = s2.ToString(),
                    Spec = WRITE,
                    Inherit = false,
                    Inherited = false,
                },
            };
            var nameResolver = new DefaultNameResolver("");
            var infoExtractor = new PermissionInfoExtractor(store, nameResolver);
            var actual = infoExtractor.GetPermissionInfo(n1_2).ToList();

            actual.Should().NotBeNull();
            actual.Count().Should().Be(expected.Count);
            foreach (var ex in expected)
            {
                actual.Should().Contain(ex);
            }
        }
    }
}
