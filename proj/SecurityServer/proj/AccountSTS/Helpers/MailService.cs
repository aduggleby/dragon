using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragon.Mail;
using System.Dynamic;
using Dragon.Mail.Impl;

namespace Dragon.SecurityServer.AccountSTS.Helpers
{
    public class MailService 
    {
        private MailSenderService m_mailSenderService;
        private MailGeneratorService m_mailGeneratorService;
        private bool m_templatesLoaded = false;

        private MailGeneratorService MailGeneratorService
        {
            get
            {
                if (!m_templatesLoaded)
                {
                    // Requires HttpContext so we can't do this in ctor.
                    var templateFolder =
                        new FileFolderTemplateRepository(HttpContext.Current.Server.MapPath("~/templates"));
                    templateFolder.EnumerateTemplates(m_mailGeneratorService.Register);
                    m_templatesLoaded = true;
                }

                return m_mailGeneratorService;
            }
            set { m_mailGeneratorService = value; }
        }

       
        public MailService()
        {
            var queue = new InMemoryMailQueue();
            m_mailSenderService = new MailSenderService(queue);
            MailGeneratorService = new MailGeneratorService(queue,
                mailSenderService: m_mailSenderService,
                async: false);
        }

        private dynamic User(UserModel u)
        {
            dynamic user = new ExpandoObject();
            user.email = u.Email;
            user.fullname = u.FullName;
            user.userid = u.UserID;
            return user;
        }

       
        public void SendWelcome(UserModel model)
        {
        }
        
        public void SendPasswordReset(UserModel recipient, string url)
        {
            dynamic data = new ExpandoObject();
            data.url = url;

            MailGeneratorService.Send(User(recipient), "PasswordReset", data);
        }

        public void SendInvalidPasswordReset(UserModel recipient)
        {
            dynamic data = new ExpandoObject();

            MailGeneratorService.Send(User(recipient), "InvalidPasswordReset", data);
        }
    }
}