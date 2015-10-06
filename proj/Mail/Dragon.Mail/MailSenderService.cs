using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Utils;

namespace Dragon.Mail
{
    public class MailSenderService : IMailSenderService
    {
        public const string APP_KEY_WAIT_ERROR = "Dragon.Mail.Async.WaitAfterError";
        public const string APP_KEY_WAIT_NOWORK = "Dragon.Mail.Async.WaitAfterNoWork";

        public const string APP_KEY_OVERRIDE_RECIPIENT = "Dragon.Mail.Override.Recipient";
        public const string APP_KEY_OVERRIDE_SMTP_HOST = "Dragon.Mail.Override.Smtp.Host";
        public const string APP_KEY_OVERRIDE_SMTP_PORT = "Dragon.Mail.Override.Smtp.Port";
        public const string APP_KEY_OVERRIDE_SMTP_SSL = "Dragon.Mail.Override.Smtp.SSL";
        public const string APP_KEY_OVERRIDE_SMTP_USER = "Dragon.Mail.Override.Smtp.User";
        public const string APP_KEY_OVERRIDE_SMTP_PASSWORD = "Dragon.Mail.Override.Smtp.Password";
        public const string APP_KEY_OVERRIDE_SMTP_DOMAIN = "Dragon.Mail.Override.Smtp.Domain";

        private IMailQueue m_queue;
        private IConfiguration m_configuration;

        private static int s_sleepAfterException = 1000;
        private static int s_sleepAfterOnNoWork = 5000;

        private static bool m_continue = false;

        private static string s_overrideRecipient = "";
        private static string s_overrideSmtpHost = "";
        private static int? s_overrideSmtpPort = null;
        private static bool? s_overrideSmtpSSL = null;
        private static string s_overrideSmtpUser = "";
        private static string s_overrideSmtpPassword = "";
        private static string s_overrideSmtpDomain = "";

        private static Thread m_thread;

        private static ILog s_log = LogManager.GetCurrentClassLogger();

        public MailSenderService(IMailQueue queue, IConfiguration configuration = null)
        {
            if (queue == null) throw new ArgumentException("Queue cannot be null.", "queue");

            m_configuration = configuration ?? new DefaultConfiguration();
            m_queue = queue;

            s_sleepAfterException = StringToIntUtil.Interpret(m_configuration.GetValue(APP_KEY_WAIT_ERROR), s_sleepAfterException);
            s_sleepAfterOnNoWork = StringToIntUtil.Interpret(m_configuration.GetValue(APP_KEY_WAIT_NOWORK), s_sleepAfterOnNoWork);

            s_overrideRecipient = m_configuration.GetValue(APP_KEY_OVERRIDE_RECIPIENT);
            s_overrideSmtpHost = m_configuration.GetValue(APP_KEY_OVERRIDE_SMTP_HOST);
            s_overrideSmtpPort = StringToIntUtil.InterpretNullable(m_configuration.GetValue(APP_KEY_OVERRIDE_SMTP_PORT), null);
            s_overrideSmtpSSL = StringToBoolUtil.InterpretNullable(m_configuration.GetValue(APP_KEY_OVERRIDE_SMTP_SSL), null);
            s_overrideSmtpUser = m_configuration.GetValue(APP_KEY_OVERRIDE_SMTP_USER);
            s_overrideSmtpPassword = m_configuration.GetValue(APP_KEY_OVERRIDE_SMTP_PASSWORD);
            s_overrideSmtpDomain = m_configuration.GetValue(APP_KEY_OVERRIDE_SMTP_DOMAIN);

        }

        public void Start()
        {
            m_continue = true;
            m_thread = new Thread(new ThreadStart(Loop));
            m_thread.Start();
        }

        public void Stop()
        {
            s_log.Debug("Signal for stop received.");
            m_continue = false;
        }

        private void Loop()
        {
            try
            {
                while (m_continue)
                {
                    try
                    {
                        if (!ProcessNext())
                        {
                            s_log.Debug("No work found.");

                            Thread.Sleep(s_sleepAfterException);
                        }
                    }
                    catch (Exception ex)
                    {
                        s_log.Error("Exception executing worker: " + ex.ToString(), ex);
                        Thread.Sleep(s_sleepAfterException);
                    }
                }
                s_log.Debug("Worker loop ended.");

                if (m_queue is IDisposable)
                {
                    s_log.Debug("Disposing queue...");
                    ((IDisposable)m_queue).Dispose();
                    s_log.Debug("Queue disposed.");
                }
            }
            catch (Exception ex)
            {
                s_log.Error("Abnormal loop end due to exception: " + ex.ToString(), ex);
            }
        }

        public bool ProcessNext()
        {
            s_log.Debug("Checking for work.");

            return m_queue.Dequeue(Process);
        }

        public bool Process(Models.RenderedMail mail)
        {
            return ProcessInternal(mail, null);
        }



        public bool ProcessInternal(Models.RenderedMail mail, ISmtpClient smtpClient = null)
        {
            try
            {
                s_log.Debug("Processing mail item.");

                var smtp = smtpClient ?? new DefaultSmtpClient();

                if (!string.IsNullOrWhiteSpace(s_overrideSmtpHost))
                {
                    smtp.SetHost(s_overrideSmtpHost);
                }
                if (s_overrideSmtpPort.HasValue)
                {
                    smtp.SetPort(s_overrideSmtpPort.Value);
                }
                if (s_overrideSmtpSSL.HasValue)
                {
                    smtp.SetEnableSsl(s_overrideSmtpSSL.Value);
                }
                if (!string.IsNullOrWhiteSpace(s_overrideSmtpUser) ||
                    !string.IsNullOrWhiteSpace(s_overrideSmtpPassword) ||
                    !string.IsNullOrWhiteSpace(s_overrideSmtpDomain))
                {
                    smtp.SetCredentials(
                        s_overrideSmtpDomain,
                        s_overrideSmtpUser,
                        s_overrideSmtpPassword);
                }

                var msg = CreateMailMessage(mail);

                if (!string.IsNullOrWhiteSpace(s_overrideRecipient))
                {
                    msg.To.Clear();
                    msg.To.Add(s_overrideRecipient);
                }

                smtp.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                s_log.Error("Error processing mail: " + ex.ToString());
                return false;
            }
        }

        public MailMessage CreateMailMessage(RenderedMail mail)
        {
            var mm = new MailMessage(mail.Sender, mail.Receiver)
            {
                Subject = mail.Subject,
            };

            if (!string.IsNullOrWhiteSpace((mail.Body ?? string.Empty).Trim()))
            {
                AlternateView htmlView =
                    AlternateView.CreateAlternateViewFromString(
                        mail.Body, null, MediaTypeNames.Text.Html);

                mm.AlternateViews.Add(htmlView);
            }

            // Use a defined text mail if we have one, otherwise use the converted html text
            string plainTextBody = mail.TextBody;

            if (string.IsNullOrWhiteSpace((plainTextBody ?? string.Empty).Trim()))
            {
                plainTextBody = HtmlToPlainText.ConvertHtml(mail.Body);
            }

            AlternateView plainTextView =
                AlternateView.CreateAlternateViewFromString(
                    plainTextBody, null, MediaTypeNames.Text.Plain);

            mm.AlternateViews.Add(plainTextView);
            return mm;
        }
    }
}
