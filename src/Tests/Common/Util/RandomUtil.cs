using System;
using Dragon.Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
namespace Dragon.Tests.Common.Util
{
    [TestClass()]
    public class RandomUtilTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Int_Boundary_0_1()
        {
            RandomUtil.Int(0, false);
           
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Int_Boundary_0_2()
        {
            RandomUtil.Int(0, true);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Int_Boundary_11_1()
        {
            RandomUtil.Int(11, false);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Int_Boundary_11_2()
        {
            RandomUtil.Int(11, true);
        }

        [TestMethod()]
        public void Int_Normal()
        {
            for (int i = 1; i < 11; i++)
            {
                RandomUtil.Int(i, false).Should().BeGreaterThan(0);
                RandomUtil.Int(i, false).ToString().Length.Should().BeLessOrEqualTo(i);
                RandomUtil.Int(i, true).ToString().Length.Should().Be(i);
            }
        }

    }
}
