using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Core.Mail;
using Dragon.Interfaces.ActivityCenter;
using Dragon.Interfaces.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Dragon.Tests.Notification.Email
{
    [TestClass]
    public class basics : _base
    {
        [TestInitialize]
        public void Setup()
        {
            // defaults
            m_getProfile = (n) => null;
            m_flushSinceActivityFor = (n, a) => null;
            m_generate = (a, st, n) => null;
            m_send = (e, s, b, h) => { };
            //m_save = (a) => { };
            //m_saveNotifiable = (a, n) => { };
            //m_getSince = (a) => null;
            //m_get = (a) => null;
        }

        [TestMethod]
        public void one_activity_no_html_no_flushing_no_backlog_sends_one_text_notification()
        {
            string email = null;
            string subject = null;
            string body = null;
            bool? html = null;

            m_send = (e, s, b, h) =>
            {
                email = e;
                subject = s;
                body = b;
                html = h;
            };

            const string temail = "me@me.com";
            m_flushSinceActivityFor = (n, a) => new IActivity[] { a }; // no backlog, no flushing
            m_getProfile = (n) => new MockProfile() { PrimaryEmailAddress = temail, WantsHtml = false };
            m_generate =
                (a, st, n) =>
                    new MockTemplateServiceResult() { Body = "mockbody", Subject = "mocksubject", Subtype = st.First() };

            CreateSubject().Notify(new MockActivity(), new MockNotifiable());

            email.Should().Be(temail);
            subject.Should().Be("mocksubject");
            body.Trim().Should().Be("mockbody");
            html.Should().Be(false);
        }

        [TestMethod]
        public void one_activity_with_html_no_flushing_no_backlog_sends_one_html_notification()
        {
            string email = null;
            string subject = null;
            string body = null;
            bool? html = null;

            m_send = (e, s, b, h) =>
            {
                email = e;
                subject = s;
                body = b;
                html = h;
            };

            const string temail = "me@me.com";
            m_flushSinceActivityFor = (n, a) => new IActivity[] { a }; // no backlog, no flushing
            m_getProfile = (n) => new MockProfile() { PrimaryEmailAddress = temail, WantsHtml = true };
            m_generate =
                (a, st, n) =>
                    new MockTemplateServiceResult() { Body = "mockbody", Subject = "mocksubject", Subtype = st.First() };

            CreateSubject().Notify(new MockActivity(), new MockNotifiable());

            email.Should().Be(temail);
            subject.Should().Be("mocksubject");
            body.Trim().Should().Be("<p>mockbody</p>");
            html.Should().Be(true);
        }
        
        [TestMethod]
        public void one_activity_with_html_with_flushing_no_backlog_sends_no_notification()
        {
            bool sent = false;

            m_send = (e, s, b, h) =>
            {
                sent = true;
            };

            const string temail = "me@me.com";
            m_flushSinceActivityFor = (n, a) => null; // no backlog, with flushing
            m_getProfile = (n) => new MockProfile() { PrimaryEmailAddress = temail, WantsHtml = true };
            m_generate =
                (a, st, n) =>
                    new MockTemplateServiceResult() { Body = "mockbody", Subject = "mocksubject", Subtype = st.First() };

            CreateSubject().Notify(new MockActivity(), new MockNotifiable());

            sent.Should().BeFalse();
        }

        [TestMethod]
        public void one_activity_with_html_with_flushing_with_backlog_sends_one_html_batch_notification()
        {
            int sent = 0;
            string body = null;

            m_send = (e, s, b, h) =>
            {
                sent++;
                body = b;
            };

            const string temail = "me@me.com";
            m_flushSinceActivityFor = (n, a) => new IActivity[] { a, new MockActivity(),  new MockActivity2() }; // no backlog, no flushing
            m_getProfile = (n) => new MockProfile() { PrimaryEmailAddress = temail, WantsHtml = true };
            m_generate =
                (a, st, n) =>
                    new MockTemplateServiceResult() { Body = new string('*',a.Count()), Subject = "mocksubject", Subtype = st.First() };

            CreateSubject().Notify(new MockActivity(), new MockNotifiable());

            sent.Should().Be(1);
            body.Trim().Should().Be(@"<p>mocksubject</p>
<p>**</p>
<p>mocksubject</p>
<p>*</p>");
        }

    }
}
