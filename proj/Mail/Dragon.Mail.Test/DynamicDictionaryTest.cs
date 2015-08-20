using System;
using System.Collections.Generic;
using Dragon.Mail.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class DynamicDictionaryTest
    {
        [TestMethod]
        public void ResolvesToDynamic()
        {
            // ARRANGE
            dynamic subject = new DynamicDictionary(
                new Dictionary<string, object>
                {
                    { "test1", 1 },
                    { "test2", new { alex = "bob" } }
                });

            // ASSERT
            Assert.AreEqual(1, subject.test1);
            Assert.AreEqual("bob", subject.test2.alex);
        }

       
    }
}
