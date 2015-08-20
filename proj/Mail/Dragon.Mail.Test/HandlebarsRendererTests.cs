using System;
using Dragon.Mail.Impl;
using Dragon.Mail.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class HandlebarsRendererTests
    {
        [TestMethod]
        public void Renders_all()
        {
            // ARRANGE
            var expected = "<strong>bob</strong>";
            var t = new Template();
            t.Content.Subject = "<strong>{{name}}</strong>";
            t.Content.Body = "<strong>{{name}}</strong>";
            t.Content.TextBody = "<strong>{{name}}</strong>";

            t.Content.SummaryBody = "<strong>{{name}}</strong>";
            t.Content.SummaryHeader = "<strong>{{name}}</strong>";
            t.Content.SummaryFooter = "<strong>{{name}}</strong>";
            t.Content.SummarySubject = "<strong>{{name}}</strong>";
            t.Content.SummaryTextBody = "<strong>{{name}}</strong>";
            t.Content.SummaryTextHeader = "<strong>{{name}}</strong>";
            t.Content.SummaryTextFooter = "<strong>{{name}}</strong>";
   
            var r = new HandlebarsRenderer(t);

            // ACT
            var mail = new Models.Mail();
            r.Render(mail, new { name = "bob" });

            // ASSERT
            Assert.AreEqual(expected, mail.Body);
            Assert.AreEqual(expected, mail.Subject);
            Assert.AreEqual(expected, mail.SummaryBody);
            Assert.AreEqual(expected, mail.SummaryFooter);
            Assert.AreEqual(expected, mail.SummaryHeader);
            Assert.AreEqual(expected, mail.SummarySubject);
            Assert.AreEqual(expected, mail.TextBody);
            Assert.AreEqual(expected, mail.SummaryTextBody);
            Assert.AreEqual(expected, mail.SummaryTextFooter);
            Assert.AreEqual(expected, mail.SummaryTextHeader);
        }



        [TestMethod]
        public void Renders_Complex()
        {
            // ARRANGE
            var expected = "<strong>alice</strong>";
            var t = new Template();
            t.Content.Body = "<strong>{{name.sub}}</strong>";
            var r = new HandlebarsRenderer(t);

            // ACT
            var mail = new Models.Mail();

            r.Render(mail, new
            {
                name = new
                    {
                        sub = "alice"
                    }
            });

            // ASSERT
            Assert.AreEqual(expected, mail.Body);
        }
    }
}
