using System.Collections.Generic;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotificationTest
{
    [TestClass]
    public class StringTemplateTemplateServiceTest
    {
        private static readonly Dictionary<string, string> Parameter = new Dictionary<string, string> { { "name", "abc" } };
        private const string TEMPLATE_TEXT = "test <name> content.";
        private const string PARSED_TEMPLATE = "test abc content.";

        [TestMethod]
        public void ParseShouldReturnParsedTemplate()
        {
            var templateService = new StringTemplateTemplateService();
            var result = templateService.Parse(TEMPLATE_TEXT, Parameter);
            Assert.AreEqual(PARSED_TEMPLATE, result);
        }
    }
}
