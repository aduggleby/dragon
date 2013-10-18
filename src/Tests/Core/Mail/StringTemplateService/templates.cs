using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Core.Mail;
using Dragon.Interfaces.ActivityCenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Globalization;

namespace Dragon.Tests.Core.Mail.StringTemplateService
{
    [TestClass]
    public class templates : _base
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
        public void generates_template_correctly_from_one_activity()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) =>
                x.Equals("MockActivity_en-us_html.st", StringComparison.CurrentCultureIgnoreCase) ||
                x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"Subject: Test for $to.Name$
Hello
$model:{it|- $it.Name$
}$";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { Name = "Alice" });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us"),
                Name = "Max"
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("html");
            tmpl.Subject.Should().Be("Test for Max");
            tmpl.Body.Should().Be(@"Hello
- Alice
");
        }

        [TestMethod]
        public void generates_template_correctly_from_two_activities()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) =>
                x.Equals("MockActivity_en-us_html.st", StringComparison.CurrentCultureIgnoreCase) ||
                x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"Subject: Test for $to.Name$
Hello
$model:{it|- $it.Name$
}$";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { Name = "Alice" });
            activities.Add(new MockActivity() { Name = "Bob" });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us"),
                Name = "Max"
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("html");
            tmpl.Subject.Should().Be("Test for Max");
            tmpl.Body.Should().Be(@"Hello
- Alice
- Bob
");
        }
    }
}
