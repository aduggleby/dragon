using System;
using Dragon.Context.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class DefaultNameResolverTest
    {
        [TestMethod]
        public void ResolveShouldReturnCorrectID()
        {
            const string prefix = "test";
            var nameResolver = new DefaultNameResolver(prefix);
            var guid = new Guid();
            var expected = prefix + guid;
            var actual = nameResolver.Resolve(guid);
            Assert.AreEqual(expected, actual);
        }
    }
}
