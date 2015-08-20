using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Dragon.Mail.Interfaces;

namespace Dragon.Mail.SqlQueue
{
    public class SqlMailQueue : IMailQueue
    {
        public const string ADDITIONAL_PARAMETER_USERID = "userid";
        public const string ADDITIONAL_PARAMETER_BUFFERHOURS = "bufferHours";
        public const string ADDITIONAL_PARAMETER_IGNOREBUFFER = "ignoreBuffer";
        public const string ADDITIONAL_PARAMETER_FLUSHBUFFER = "flushBuffer";

        private IRepository<Mail> m_mails;

        public SqlMailQueue(IRepository<Mail> mails = null)
        {
            m_mails = mails ?? new Repository<Mail>();
        }

        public void Enqueue(Models.Mail mail, dynamic additionalParameters)
        {
            var mailRow = new Mail();

            mailRow.MailID = Guid.NewGuid();
            mailRow.Sent = false;
            mailRow.Error = false;
            mailRow.SenderDisplayName = mail.Sender.DisplayName;
            mailRow.SenderEmailAddress = mail.Sender.Address;
            mailRow.ReceiverDisplayName = mail.Receiver.DisplayName;
            mailRow.ReceiverEmailAddress = mail.Receiver.Address;
            mailRow.Subject = mail.Subject;
            mailRow.Body = mail.Body;
            mailRow.SummaryBody = mail.SummaryBody;
            mailRow.SummarySubject = mail.SummarySubject;
            mailRow.SummaryHeader = mail.SummaryHeader;
            mailRow.SummaryFooter = mail.SummaryFooter;

            mailRow.EnqueuedUTC = DateTime.UtcNow;
            mailRow.NextProcessingAttemptUTC = DateTime.UtcNow;
            mailRow.LastProcessedUTC = null;

            SetIfHasKey<string>((object)additionalParameters, ADDITIONAL_PARAMETER_USERID, x => mailRow.UserID = x);
            SetIfHasKey<bool>((object)additionalParameters, ADDITIONAL_PARAMETER_FLUSHBUFFER,
                x => mailRow.BufferFlush = x);
            SetIfHasKey<int>((object)additionalParameters, ADDITIONAL_PARAMETER_BUFFERHOURS,
                x => mailRow.BufferHours = x);
            SetIfHasKey<bool>((object)additionalParameters, ADDITIONAL_PARAMETER_IGNOREBUFFER,
                x => mailRow.BufferIgnored = x);

            m_mails.Insert(mailRow);
        }

        private void SetIfHasKey<T>(object p, string key, Action<T> set)
        {
            if (p is IDictionary<string, object>)
            {
                var exp = (IDictionary<string, object>)p;
                var hasKey = exp.ContainsKey(key);
                if (hasKey)
                {
                    var val = exp[key];
                    set((T)val);
                }
            }
            else
            {
                var prop = p.GetType().GetProperty(key);
                var hasKey = prop != null;
                if (hasKey)
                {
                    var val = prop.GetValue(p, null);
                    set((T)val);
                }
            }
        }

        public bool Dequeue(Func<Models.RenderedMail, bool> processor)
        {
            return Dequeue(processor, GetOldestBatch, GetLastSentForUser);
        }

        public bool Dequeue(Func<Models.RenderedMail, bool> processor,
                Func<IEnumerable<Mail>> getOldestBatch,
                Func<string, Mail> getLastSentForUser)
        {
            // ignorebuffer - send.
            // bufferhours = 0 or flushbuffer = true - flushsend
            // bufferhours > 0 - get last sent

            using (var tx = new TransactionScope())
            {
                var mailRows = getOldestBatch();
                if (mailRows.Any())
                {
                    // 1. Send those that ignore buffer one by one
                    var sendSingle = mailRows.Where(x => x.BufferIgnored);
                    foreach (var mailRow in sendSingle)
                    {
                        SendAndUpdateMailRow(processor, mailRow);
                    }

                    // 2. Evaluate if its time to send a batch
                    var lastEnqueued = mailRows.OrderByDescending(x => x.EnqueuedUTC).FirstOrDefault();
                    lastEnqueued.LastProcessedUTC = DateTime.UtcNow;

                    var lastSentRow = getLastSentForUser(lastEnqueued.UserID);

                    var lastSentUtc = DateTime.MinValue;
                    if (lastSentRow != null) lastSentUtc = lastSentRow.SentUTC.Value;

                    var earliestSendUtc = lastSentUtc.AddHours(lastEnqueued.BufferHours);
                    var canSendBuffered = earliestSendUtc <= DateTime.UtcNow || lastEnqueued.BufferFlush;

                    if (canSendBuffered)
                    {
                        // If it's time, send all open email as a summary email
                        var mailsToSend = mailRows.Where(x => !x.BufferIgnored);
                        SendAndUpdateBuffer(processor, mailsToSend);
                    }
                    else
                    {
                        // If not, push earliest to at least the next possible send date
                        lastEnqueued.NextProcessingAttemptUTC = earliestSendUtc;
                        m_mails.Update(lastEnqueued);
                    }

                    tx.Complete();

                    return true;
                }
                else
                {
                    // no mail rows
                    return false;
                }
            }
        }

        private Mail GetLastSentForUser(string userID)
        {
            var lastSentRow = m_mails.Query(SQL.GET_LASTSENT_FORUSER, new { UserID = userID }).FirstOrDefault();
            return lastSentRow;
        }

        private IEnumerable<Mail> GetOldestBatch()
        {
            var mailRows = m_mails.Query(SQL.GET_EARLIEST);
            return mailRows;
        }

        private void SendAndUpdateMailRow(Func<Models.RenderedMail, bool> processor, Mail mailRow)
        {
            var mail = new Models.RenderedMail();

            mail.Sender = new System.Net.Mail.MailAddress(
                mailRow.SenderEmailAddress,
                mailRow.SenderDisplayName);

            mail.Receiver = new System.Net.Mail.MailAddress(
                mailRow.ReceiverEmailAddress,
                mailRow.ReceiverDisplayName);

            mail.Subject = mailRow.Subject;
            mail.Body = mailRow.Body;
            mail.TextBody = mailRow.TextBody;
            
            var success = false;
            try
            {
                success = processor(mail);
            }
            catch (Exception ex)
            {
                success = false;
            }
            mailRow.Sent = true;
            mailRow.SentUTC = DateTime.UtcNow;
            mailRow.Error = !success;
            m_mails.Update(mailRow);
        }

        private void SendAndUpdateBuffer(Func<Models.RenderedMail, bool> processor, IEnumerable<Mail> mailRows)
        {
            var mail = new Models.RenderedMail();

            var newestMailRow = mailRows.OrderByDescending(x => x.EnqueuedUTC).First();

            mail.Sender = new System.Net.Mail.MailAddress(
                newestMailRow.SenderEmailAddress,
                newestMailRow.SenderDisplayName);

            mail.Receiver = new System.Net.Mail.MailAddress(
                newestMailRow.ReceiverEmailAddress,
                newestMailRow.ReceiverDisplayName);

            mail.Subject = newestMailRow.SummarySubject ?? string.Empty;
            if (string.IsNullOrWhiteSpace(mail.Subject.Trim()))
            {
                mail.Subject = "Notifications";
            }

            var body = new StringBuilder();
            body.AppendLine(newestMailRow.SummaryHeader);
            foreach (var mailRow in mailRows)
            {
                body.AppendLine(mailRow.SummaryBody);
            }
            body.AppendLine(newestMailRow.SummaryFooter);

            mail.Body = body.ToString();


            var textbody = new StringBuilder();
            textbody.AppendLine(newestMailRow.SummaryTextHeader);
            foreach (var mailRow in mailRows)
            {
                textbody.AppendLine(mailRow.SummaryTextBody);
            }
            textbody.AppendLine(newestMailRow.SummaryTextFooter);

            mail.TextBody = textbody.ToString();

            var success = false;
            try
            {
                success = processor(mail);
            }
            catch (Exception ex)
            {
                success = false;
            }


            foreach (var mailRow in mailRows)
            {
                mailRow.Sent = true;
                mailRow.SentUTC = DateTime.UtcNow;
                mailRow.Error = !success;
                m_mails.Update(mailRow);
            }
        }
    }
}
