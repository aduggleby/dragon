using System;
using Dragon.Context.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class DefaultNameResolverTest
    {
        [TestMethod]
        public void ResolveSubjectShouldReturnCorrectID()
        {
            const string prefix1 = "test";
            const string prefix2 = "test2";
            var nameResolver = new DefaultNameResolver(prefix1, prefix2);
            var guid = new Guid();
            var expected = prefix1 + guid;
            var actual = nameResolver.ResolveSubjectID(guid);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResolveNodeShouldReturnCorrectID()
        {
            const string prefix1 = "test";
            const string prefix2 = "test2";
            var nameResolver = new DefaultNameResolver(prefix1, prefix2);
            var guid = new Guid();
            var expected = prefix2 + guid;
            var actual = nameResolver.ResolveNodeID(guid);
            Assert.AreEqual(expected, actual);
        }
    }
}
