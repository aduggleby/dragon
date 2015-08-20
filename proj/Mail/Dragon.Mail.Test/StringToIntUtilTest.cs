using System;
using Dragon.Mail.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class StringToIntUtilTest
    {
        [TestMethod]
        public void Basic()
        {
            Assert.AreEqual(-9999, StringToIntUtil.Interpret("-9999", -1));
            Assert.AreEqual(-1, StringToIntUtil.Interpret("-1", -1));
            Assert.AreEqual(0, StringToIntUtil.Interpret("0", -1));
            Assert.AreEqual(1, StringToIntUtil.Interpret("1", -1));
            Assert.AreEqual(9999, StringToIntUtil.Interpret("9999", -1));
            Assert.AreEqual(9999, StringToIntUtil.Interpret("0009999", -1));

            Assert.AreEqual(-1, StringToIntUtil.Interpret("abc", -1));
            Assert.AreEqual(-1, StringToIntUtil.Interpret("4-1", -1));
            Assert.AreEqual(-1, StringToIntUtil.Interpret("4,1", -1));
            Assert.AreEqual(-1, StringToIntUtil.Interpret("4.1", -1));
            Assert.AreEqual(-1, StringToIntUtil.Interpret("-1a", -1));

           
        }
    }
}
