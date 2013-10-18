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
    public class basics : _base
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
        public void no_activities_throws_error()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => false;
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_TEXT, notifiable);
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void different_activities_throws_error()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => false;
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });
            activities.Add(new MockActivity2() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_TEXT, notifiable);
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void does_not_find_template_and_throws_error()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => false;
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_TEXT, notifiable);
        }

        [TestMethod]
        public void finds_template_and_generates_template_for_one_activity_without_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() {});

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_TEXT, notifiable);

            tmpl.Subtype.Should().Be("text");
            tmpl.Subject.Should().Be(null);
            tmpl.Body.Should().Be(@"I am a basic template");
        }
        
        [TestMethod]
        public void finds_template_and_generates_template_for_one_activity_with_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"Subject: Test
I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_TEXT, notifiable);

            tmpl.Subtype.Should().Be("text");
            tmpl.Subject.Should().Be("Test");
            tmpl.Body.Should().Be(@"I am a basic template");
        }

        [TestMethod]
        public void finds_first_template_from_subtypes_and_generates_template_for_one_activity_without_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => x.Equals("MockActivity_en-us_html.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("html");
            tmpl.Subject.Should().Be(null);
            tmpl.Body.Should().Be(@"I am a basic template");
        }

        [TestMethod]
        public void finds_first_template_from_subtypes_and_generates_template_for_one_activity_with_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => x.Equals("MockActivity_en-us_html.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"Subject: Test
I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("html");
            tmpl.Subject.Should().Be("Test");
            tmpl.Body.Should().Be(@"I am a basic template");
        }

        [TestMethod]
        public void finds_second_template_from_subtypes_and_generates_template_for_one_activity_without_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("text");
            tmpl.Subject.Should().Be(null);
            tmpl.Body.Should().Be(@"I am a basic template");
        }

        [TestMethod]
        public void finds_second_template_from_subtypes_and_generates_template_for_one_activity_with_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"Subject: Test
I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("text");
            tmpl.Subject.Should().Be("Test");
            tmpl.Body.Should().Be(@"I am a basic template");
        }

        [TestMethod]
        public void finds_both_templates_from_subtypes_and_generates_template_for_one_activity_without_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) => 
                x.Equals("MockActivity_en-us_html.st", StringComparison.CurrentCultureIgnoreCase) ||
                x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("html");
            tmpl.Subject.Should().Be(null);
            tmpl.Body.Should().Be(@"I am a basic template");
        }

        [TestMethod]
        public void finds_both_templates_from_subtypes_and_generates_template_for_one_activity_with_subject()
        {
            m_existsDir = (x) => true;
            m_existsFile = (x) =>
                x.Equals("MockActivity_en-us_html.st", StringComparison.CurrentCultureIgnoreCase) ||
                x.Equals("MockActivity_en-us_text.st", StringComparison.CurrentCultureIgnoreCase);
            m_fileContents = (x) => @"Subject: Test
I am a basic template";

            var subject = new Antlr4StringTemplateService("", this, this);

            var activities = new List<IActivity>();
            activities.Add(new MockActivity() { });

            var notifiable = new MockNotifiable()
            {
                PrimaryCulture = CultureInfo.GetCultureInfo("en-us")
            };

            var tmpl = subject.Generate(activities, SUBTYPE_HTML_TEXT, notifiable);

            tmpl.Subtype.Should().Be("html");
            tmpl.Subject.Should().Be("Test");
            tmpl.Body.Should().Be(@"I am a basic template");
        }
    }
}
