using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Dragon.ActivityCenter.Notification;
using Dragon.Interfaces;
using Dragon.Interfaces.ActivityCenter;
using Dragon.Interfaces.Core;

namespace Dragon.Tests.Notification.Email
{
    public class _base : IEmailService, IEmailTemplateService, IEmailProfileService
    {
        protected Func<INotifiable, IEmailProfile> m_getProfile;
        protected Func<INotifiable, IActivity, IEnumerable<IActivity>> m_flushSinceActivityFor;
        protected Func<IEnumerable<IActivity>, string[], INotifiable, ITemplateServiceResult> m_generate;
        protected Action<string, string, string, bool> m_send;
        protected Action<string, string, string, bool, Dictionary<string, byte[]>> m_sendWithAttachments;

        //protected Action<IActivity, INotifiable> m_saveNotifiable;
        //protected Action<IActivity> m_save;
        //protected Func<IActivity, IEnumerable<IActivity>> m_getSince;
        //protected Func<IActivity, IActivity> m_get;
        
        protected EmailNotificationDispatcher CreateSubject()
        {
            return new EmailNotificationDispatcher(this, this, this);
        }

        //public IActivity Get(IActivity template)
        //{
        //    return m_get(template);
        //}

        //public IEnumerable<IActivity> GetSince(IActivity activity)
        //{
        //    return m_getSince(activity);
        //}

        //public void Save(IActivity activity)
        //{
        //    m_save(activity);
        //}

        //public void SaveNotifiable(IActivity activity, INotifiable notifiable)
        //{
        //    m_saveNotifiable(activity, notifiable);
        //}
        
        public void Send(string to, string subject, string body, bool useHtmlEmail)
        {
            m_send(to,subject,body,useHtmlEmail);
        }

        public void Send(string to, string subject, string body, bool useHtmlEmail, Dictionary<string, byte[]> attachments)
        {
            m_sendWithAttachments(to, subject, body, useHtmlEmail, attachments);
        }

        public ITemplateServiceResult Generate(string type, string[] subtypeOrder, string culture, Dictionary<string, object> model)
        {
            throw new NotImplementedException();
        }

        public ITemplateServiceResult Generate(IEnumerable<IActivity> activity, string[] subtypeOrder, INotifiable notifiable)
        {
            return m_generate(activity, subtypeOrder, notifiable);
        }

        public IEnumerable<IActivity> AppendAndFlushIfNecessary(INotifiable notifiable, IActivity activity)
        {
            return m_flushSinceActivityFor(notifiable, activity);
        }
        
        public IEmailProfile GetProfile(INotifiable notifiable)
        {
            return m_getProfile(notifiable);
        }
    }

    public class MockTemplateServiceResult : ITemplateServiceResult
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Subtype { get; set; }
    }

    public class MockActivity : IActivity, IEmailNotificationActivity
    {
        public string Name { get; set; }
    }

    public class MockActivity2 : IActivity, IEmailNotificationActivity
    {

    }

    public class MockNotifiable : INotifiable
    {
        public CultureInfo PrimaryCulture { get; set; }
        public string Name { get; set; }
    }

    public class MockProfile : IEmailProfile
    {
        public string PrimaryEmailAddress { get; set; }
        public bool WantsHtml { get; set; }
    }

}
