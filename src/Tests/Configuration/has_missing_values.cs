using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Common.Extensions;
using Dragon.Core.Configuration;
using Dragon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Dragon.Tests.Configuration
{
    [TestClass]
    public class has_missing_values
    {
        private IConfiguration m_config;

        [TestInitialize]
        public void Setup()
        {
            m_config = new InMemoryConfiguration();
        }

        [TestMethod]
        public void and_get_returns_defaults()
        {
            m_config.GetValue<string>("unknown", null).Should().BeNull();
            m_config.GetValue<string>("unknown", "123").Should().Be("123");

            m_config.GetString("unknown", null).Should().BeNull();
            m_config.GetString("unknown", "123").Should().Be("123");

            m_config.GetInt("unknown", 123).Should().Be(123);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void and_ensure_throws_exception_if_key_does_not_exist()
        {
            m_config.EnsureValue<string>("unknown");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void and_ensure_throws_exception_if_key_does_not_exist2()
        {
            m_config.EnsureString("unknown");
        }
    }
}
