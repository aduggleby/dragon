using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Xml.XPath;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Test.Mocks;
using Dragon.Mail.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Globalization;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class ResourceFileTemplateRepositoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InvalidCultureThrowsException()
        {
            // ARRANGE
            var resMgrMock = new Mock<IResourceManager>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(ResourceFileTemplateRepository.APP_KEY_DEFLANG))
                .Returns(@"does-not-exist-as-a-culture");

            // ACT
            var queue = new ResourceFileTemplateRepository(
                configuration: configMock.Object,
                resMgr: resMgrMock.Object);

            // ASSERT
        }

        [TestMethod]
        public void Enumerate_LoadsTextKeysCorrectly()
        {
            // ARRANGE
            var cultureEN = CultureInfo.CreateSpecificCulture("en-US");
            var cultureDE = CultureInfo.CreateSpecificCulture("de-DE");
            var cultureFR = CultureInfo.CreateSpecificCulture("fr-FR");
            var cultures = new CultureInfo[] { cultureEN, cultureDE, cultureFR };

            var keys = new string[] {
                "tmpl1_body_text",
                "tmpl1_subject_text",
                "tmpl1_summarysubject_text",
                "tmpl1_summarybody_text",
                "tmpl1_summaryheader_text",
                "tmpl1_summaryfooter_text",

                "tmpl2_body_text",
                "tmpl2_subject_text",
                "tmpl2_summarysubject_text",
                "tmpl2_summarybody_text",
                "tmpl2_summaryheader_text",
                "tmpl2_summaryfooter_text",

                "tmpl3_body_text",
                "tmpl3_subject_text",
                "tmpl3_summarysubject_text",
                "tmpl3_summarybody_text",
                "tmpl3_summaryheader_text",
                "tmpl3_summaryfooter_text",
                };

            var resMgrMock = new Mock<IResourceManager>();
            resMgrMock.Setup(m => m.GetAvailableCultures()).Returns(cultures);
            resMgrMock.Setup(m => m.GetKeys(It.Is<CultureInfo>(c => c == null))).Returns(keys);
            resMgrMock.Setup(m => m.GetString(
                    It.Is<string>(s => keys.Contains(s)),
                    It.Is<CultureInfo>(c => c.Equals(cultureEN) || c.Equals(cultureDE) || c.Equals(cultureFR) ))
                )
                .Returns<string, CultureInfo>((key, ci) => $"test-text-{key}-{ci.TwoLetterISOLanguageName}");

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(ResourceFileTemplateRepository.APP_KEY_DEFLANG))
                .Returns("en-US");

            // ACT
            var queue = new ResourceFileTemplateRepository(
                configuration: configMock.Object,
                resMgr: resMgrMock.Object);

            var templates = new List<Template>();
            queue.EnumerateTemplates(templates.Add);

            // ASSERT
            Assert.AreEqual(9, templates.Count);
            
            var firstTemplate = templates[0];
            Assert.AreEqual(firstTemplate.Key, "tmpl1");
            Assert.AreEqual(firstTemplate.Language, cultureEN);
            Assert.AreEqual(firstTemplate.Content.Subject, "test-text-tmpl1_subject_text-en");
            Assert.AreEqual(firstTemplate.Content.TextBody, "test-text-tmpl1_body_text-en");

            Assert.AreEqual(firstTemplate.Content.SummarySubject, "test-text-tmpl1_summarysubject_text-en");
            Assert.AreEqual(firstTemplate.Content.SummaryTextBody, "test-text-tmpl1_summarybody_text-en");
            Assert.AreEqual(firstTemplate.Content.SummaryTextFooter, "test-text-tmpl1_summaryfooter_text-en");
            Assert.AreEqual(firstTemplate.Content.SummaryTextHeader, "test-text-tmpl1_summaryheader_text-en");

            Assert.AreEqual(firstTemplate.Content.Body, null);
            Assert.AreEqual(firstTemplate.Content.SummaryBody, null);
            Assert.AreEqual(firstTemplate.Content.SummaryFooter, null);
            Assert.AreEqual(firstTemplate.Content.SummaryHeader, null);


            var secondTemplate = templates[1];
            Assert.AreEqual(secondTemplate.Key, "tmpl2");
            Assert.AreEqual(secondTemplate.Language, cultureEN);
            Assert.AreEqual(secondTemplate.Content.Subject, "test-text-tmpl2_subject_text-en");
            Assert.AreEqual(secondTemplate.Content.TextBody, "test-text-tmpl2_body_text-en");

            Assert.AreEqual(secondTemplate.Content.SummarySubject, "test-text-tmpl2_summarysubject_text-en");
            Assert.AreEqual(secondTemplate.Content.SummaryTextBody, "test-text-tmpl2_summarybody_text-en");
            Assert.AreEqual(secondTemplate.Content.SummaryTextFooter, "test-text-tmpl2_summaryfooter_text-en");
            Assert.AreEqual(secondTemplate.Content.SummaryTextHeader, "test-text-tmpl2_summaryheader_text-en");

            var thirdTemplate = templates[2];
            Assert.AreEqual(thirdTemplate.Key, "tmpl3");
            Assert.AreEqual(thirdTemplate.Language, cultureEN);
            Assert.AreEqual(thirdTemplate.Content.Subject, "test-text-tmpl3_subject_text-en");
            Assert.AreEqual(thirdTemplate.Content.TextBody, "test-text-tmpl3_body_text-en");

            Assert.AreEqual(thirdTemplate.Content.SummarySubject, "test-text-tmpl3_summarysubject_text-en");
            Assert.AreEqual(thirdTemplate.Content.SummaryTextBody, "test-text-tmpl3_summarybody_text-en");
            Assert.AreEqual(thirdTemplate.Content.SummaryTextFooter, "test-text-tmpl3_summaryfooter_text-en");
            Assert.AreEqual(thirdTemplate.Content.SummaryTextHeader, "test-text-tmpl3_summaryheader_text-en");

            var fourthTemplate = templates[3];
            Assert.AreEqual(fourthTemplate.Key, "tmpl1");
            Assert.AreEqual(fourthTemplate.Language, cultureDE);
            Assert.AreEqual(fourthTemplate.Content.Subject, "test-text-tmpl1_subject_text-de");
            Assert.AreEqual(fourthTemplate.Content.TextBody, "test-text-tmpl1_body_text-de");

            Assert.AreEqual(fourthTemplate.Content.SummarySubject, "test-text-tmpl1_summarysubject_text-de");
            Assert.AreEqual(fourthTemplate.Content.SummaryTextBody, "test-text-tmpl1_summarybody_text-de");
            Assert.AreEqual(fourthTemplate.Content.SummaryTextFooter, "test-text-tmpl1_summaryfooter_text-de");
            Assert.AreEqual(fourthTemplate.Content.SummaryTextHeader, "test-text-tmpl1_summaryheader_text-de");


        }

        [TestMethod]
        public void Enumerate_LoadsTextAndHtmlFilesCorrectlyUsingFallback()
        {
            // ARRANGE
            var cultureEN = CultureInfo.CreateSpecificCulture("en-US");
            var cultureDE = CultureInfo.CreateSpecificCulture("de-DE");
            var cultureFR = CultureInfo.CreateSpecificCulture("fr-FR");
            var cultures = new CultureInfo[] { cultureEN, cultureDE, cultureFR };

            var keys = new string[] {
                "tmpl1_body_html",
                "tmpl1_subject_html",
                "tmpl1_summarysubject_html",
                "tmpl1_summarybody_html",
                "tmpl1_summaryheader_html",
                "tmpl1_summaryfooter_html",

                "tmpl2_body_html",
                "tmpl2_subject_html",
                "tmpl2_summarysubject_html",
                "tmpl2_summarybody_html",
                "tmpl2_summaryheader_html",
                "tmpl2_summaryfooter_html",

                "tmpl3_body_html",
                "tmpl3_subject_html",
                "tmpl3_summarysubject_html",
                "tmpl3_summarybody_html",
                "tmpl3_summaryheader_html",
                "tmpl3_summaryfooter_html",
                };

            var resMgrMock = new Mock<IResourceManager>();
            resMgrMock.Setup(m => m.GetAvailableCultures()).Returns(cultures);
            resMgrMock.Setup(m => m.GetKeys(It.Is<CultureInfo>(c => c == null))).Returns(keys);
            resMgrMock.Setup(m => m.GetString(
                    It.Is<string>(s => keys.Contains(s)),
                    It.Is<CultureInfo>(c => c.Equals(cultureEN) || c.Equals(cultureDE) || c.Equals(cultureFR)))
                )
                .Returns<string, CultureInfo>((key, ci) => $"@inc(incl)-test-text-{key}-{ci.TwoLetterISOLanguageName}");
            resMgrMock.Setup(m => m.GetString(
                    It.Is<string>(s => s=="incl"),
                    It.Is<CultureInfo>(c => c.Equals(cultureEN) || c.Equals(cultureDE) || c.Equals(cultureFR)))
                )
                .Returns<string, CultureInfo>((key, ci) => $"include-{ci.TwoLetterISOLanguageName}");

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(ResourceFileTemplateRepository.APP_KEY_DEFLANG))
                .Returns("en-US");

            // ACT
            var queue = new ResourceFileTemplateRepository(
                configuration: configMock.Object,
                resMgr: resMgrMock.Object);

            var templates = new List<Template>();
            queue.EnumerateTemplates(templates.Add);

            // ASSERT
            Assert.AreEqual(9, templates.Count);

            var firstTemplate = templates[0];
            Assert.AreEqual(firstTemplate.Key, "tmpl1");
            Assert.AreEqual(firstTemplate.Language, cultureEN);
            Assert.AreEqual(firstTemplate.Content.Subject, "include-en-test-text-tmpl1_subject_html-en");
            Assert.AreEqual(firstTemplate.Content.Body, "include-en-test-text-tmpl1_body_html-en");

            Assert.AreEqual(firstTemplate.Content.SummarySubject, "include-en-test-text-tmpl1_summarysubject_html-en");
            Assert.AreEqual(firstTemplate.Content.SummaryBody, "include-en-test-text-tmpl1_summarybody_html-en");
            Assert.AreEqual(firstTemplate.Content.SummaryFooter, "include-en-test-text-tmpl1_summaryfooter_html-en");
            Assert.AreEqual(firstTemplate.Content.SummaryHeader, "include-en-test-text-tmpl1_summaryheader_html-en");

            var secondTemplate = templates[1];
            Assert.AreEqual(secondTemplate.Key, "tmpl2");
            Assert.AreEqual(secondTemplate.Language, cultureEN);
            Assert.AreEqual(secondTemplate.Content.Subject, "include-en-test-text-tmpl2_subject_html-en");
            Assert.AreEqual(secondTemplate.Content.Body, "include-en-test-text-tmpl2_body_html-en");

            Assert.AreEqual(secondTemplate.Content.SummarySubject, "include-en-test-text-tmpl2_summarysubject_html-en");
            Assert.AreEqual(secondTemplate.Content.SummaryBody, "include-en-test-text-tmpl2_summarybody_html-en");
            Assert.AreEqual(secondTemplate.Content.SummaryFooter, "include-en-test-text-tmpl2_summaryfooter_html-en");
            Assert.AreEqual(secondTemplate.Content.SummaryHeader, "include-en-test-text-tmpl2_summaryheader_html-en");

            var thirdTemplate = templates[2];
            Assert.AreEqual(thirdTemplate.Key, "tmpl3");
            Assert.AreEqual(thirdTemplate.Language, cultureEN);
            Assert.AreEqual(thirdTemplate.Content.Subject, "include-en-test-text-tmpl3_subject_html-en");
            Assert.AreEqual(thirdTemplate.Content.Body, "include-en-test-text-tmpl3_body_html-en");

            Assert.AreEqual(thirdTemplate.Content.SummarySubject, "include-en-test-text-tmpl3_summarysubject_html-en");
            Assert.AreEqual(thirdTemplate.Content.SummaryBody, "include-en-test-text-tmpl3_summarybody_html-en");
            Assert.AreEqual(thirdTemplate.Content.SummaryFooter, "include-en-test-text-tmpl3_summaryfooter_html-en");
            Assert.AreEqual(thirdTemplate.Content.SummaryHeader, "include-en-test-text-tmpl3_summaryheader_html-en");

            var fourthTemplate = templates[3];
            Assert.AreEqual(fourthTemplate.Key, "tmpl1");
            Assert.AreEqual(fourthTemplate.Language, cultureDE);
            Assert.AreEqual(fourthTemplate.Content.Subject, "include-de-test-text-tmpl1_subject_html-de");
            Assert.AreEqual(fourthTemplate.Content.Body, "include-de-test-text-tmpl1_body_html-de");

            Assert.AreEqual(fourthTemplate.Content.SummarySubject, "include-de-test-text-tmpl1_summarysubject_html-de");
            Assert.AreEqual(fourthTemplate.Content.SummaryBody, "include-de-test-text-tmpl1_summarybody_html-de");
            Assert.AreEqual(fourthTemplate.Content.SummaryFooter, "include-de-test-text-tmpl1_summaryfooter_html-de");
            Assert.AreEqual(fourthTemplate.Content.SummaryHeader, "include-de-test-text-tmpl1_summaryheader_html-de");


        }
    }
}
