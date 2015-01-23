using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Core.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;


namespace Dragon.Tests.Core.Mail.StringTemplateService
{
    [TestClass]
    public class not_initialized_properly : _base
    {
        [TestInitialize]
        public void Setup()
        {
            // defaults
            m_existsDir = (x) => false;
            m_existsFile = (x) => false;
            m_fileContents = (x) => null;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void cant_find_basedir_throws_exception()
        {
            m_existsDir = (x) => false;
            var subject = new Antlr4StringTemplateService("x", this, this);
        }

        [TestMethod]
        public void can_find_basedir_does_not_throw_exception()
        {
            m_existsDir = (x) => x=="x";
            var subject = new Antlr4StringTemplateService("x", this, this);
        }

        [TestMethod]
        public void not_running_in_httpcontext_without_tilde_throws_no_error()
        {
            m_existsDir = (x) => x == "x";
            var subject = new Antlr4StringTemplateService("x", this, this);
        }

        [TestMethod]
        public void not_running_in_httpcontext_with_tilde_leads_to_invalid_configuration()
        {
            m_existsDir = (x) => x == "~/x";
            var subject = new Antlr4StringTemplateService("~/x", this, this);
            subject.ConfigurationOK.Should().BeTrue();
        }
        
    }
}
