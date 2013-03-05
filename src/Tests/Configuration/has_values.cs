using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Core.Configuration;
using Dragon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Dragon.Common.Extensions;

namespace Dragon.Tests.Configuration
{
    [TestClass]
    public class has_values
    {
        private InMemoryConfiguration m_config;

        [TestInitialize]
        public void Setup()
        {
            m_config = new InMemoryConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void and_setting_value_twice_results_in_exception()
        {
            m_config.Set("abc", "123");
            m_config.Set("abc", "345");

        }

        [TestMethod]
        public void and_returns_correct_values()
        {
            m_config.Set("abc", "123");
            m_config.Set("key1", "def");

            m_config.GetValue<string>("abc", null).Should().Be("123");
            m_config.GetValue<string>("key1", null).Should().Be("def");

            m_config.EnsureValue<string>("abc").Should().Be("123");
            m_config.EnsureValue<string>("key1").Should().Be("def");

            m_config.EnsureString("abc").Should().Be("123");

        }

        [TestMethod]
        public void and_getvalue_returns_default_without_default_specified()
        {
            m_config.GetValue<string>("abc").Should().BeNull();
        }

        [TestMethod]
        public void and_getstring_converts_correctly()
        {
            m_config.Set("abc", "123");
            m_config.Set("key1", "def");

            m_config.GetString("abc").Should().Be("123");
            m_config.GetString("key1").Should().Be("def");
        }

        [TestMethod]
        public void and_getint_converts_correctly()
        {
            m_config.Set("abc", "123");
            m_config.GetInt("abc").Should().Be(123);
        }

        [TestMethod]
        public void and_getbool_converts_correctly()
        {
            m_config.Set("t1", "true");
            m_config.Set("t2", "t");
            m_config.Set("t3", "T");
            m_config.Set("t4", "TRUE");
            m_config.Set("t5", "y");
            m_config.Set("t6", "yes");
            m_config.Set("t7", "YeS");
            m_config.Set("t8", "Y");
            m_config.Set("t9", "1");
            m_config.Set("t10", "1");

            m_config.Set("f1", "false");
            m_config.Set("f2", "f");
            m_config.Set("f3", "Tr");
            m_config.Set("f4", "no");
            m_config.Set("f5", "maybe");
            m_config.Set("f6", "wrong");
            m_config.Set("f7", "f");
            m_config.Set("f8", "F");
            m_config.Set("f9", "2");
            m_config.Set("f10", "0");

            m_config.IsTrue("t1").Should().BeTrue();
            m_config.IsTrue("t2").Should().BeTrue();
            m_config.IsTrue("t3").Should().BeTrue();
            m_config.IsTrue("t4").Should().BeTrue();
            m_config.IsTrue("t5").Should().BeTrue();
            m_config.IsTrue("t6").Should().BeTrue();
            m_config.IsTrue("t7").Should().BeTrue();
            m_config.IsTrue("t8").Should().BeTrue();
            m_config.IsTrue("t9").Should().BeTrue();
            m_config.IsTrue("t10").Should().BeTrue();

            m_config.IsFalse("f1").Should().BeTrue();
            m_config.IsFalse("f2").Should().BeTrue();
            m_config.IsFalse("f3").Should().BeTrue();
            m_config.IsFalse("f4").Should().BeTrue();
            m_config.IsFalse("f5").Should().BeTrue();
            m_config.IsFalse("f6").Should().BeTrue();
            m_config.IsFalse("f7").Should().BeTrue();
            m_config.IsFalse("f8").Should().BeTrue();
            m_config.IsFalse("f9").Should().BeTrue();
            m_config.IsFalse("f10").Should().BeTrue();


        }
    }
}
