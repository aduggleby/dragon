using System;
using Dragon.Mail.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class StringToBoolUtilTest
    {
        [TestMethod]
        public void Basic()
        {
            Assert.IsTrue(StringToBoolUtil.Interpret("true"));
            Assert.IsTrue(StringToBoolUtil.Interpret("TRUE"));
            Assert.IsTrue(StringToBoolUtil.Interpret("T"));
            Assert.IsTrue(StringToBoolUtil.Interpret("t"));
            Assert.IsTrue(StringToBoolUtil.Interpret("1"));
            Assert.IsTrue(StringToBoolUtil.Interpret("on"));

            Assert.IsFalse(StringToBoolUtil.Interpret("false"));
            Assert.IsFalse(StringToBoolUtil.Interpret("FALSE"));
            Assert.IsFalse(StringToBoolUtil.Interpret("F"));
            Assert.IsFalse(StringToBoolUtil.Interpret("f"));
            Assert.IsFalse(StringToBoolUtil.Interpret("0"));
            Assert.IsFalse(StringToBoolUtil.Interpret("off"));
        }
    }
}
