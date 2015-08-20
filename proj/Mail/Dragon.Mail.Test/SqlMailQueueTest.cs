using System;
using System.Collections.Generic;
using System.Linq;
using Dragon.Data.Interfaces;
using Dragon.Mail.SqlQueue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MAIL = Dragon.Mail.SqlQueue.Mail;
namespace Dragon.Mail.Test
{
    [TestClass]
    public class SqlMailQueueTest
    {
        [TestMethod]
        public void IgnoreBuffer_SendsEmailRegardless()
        {
            // ARRANGE
            var batch = GetBatch();
            batch.Add(new MAIL()
            {
                UserID = "example-user",
                SenderEmailAddress = "sender@example.org",
                ReceiverEmailAddress = "receiver@example.org",
                EnqueuedUTC = DateTime.UtcNow,
                NextProcessingAttemptUTC = DateTime.UtcNow,
                BufferIgnored = true,
                BufferHours = 4,
                Subject = "subject",
                Body = "body",
                SummaryHeader = "Welcome,",
                SummaryFooter = "Thanks",
                SummarySubject = "summary",
                SummaryBody = "summarybody"
            });

            var repMock = new Mock<IRepository<MAIL>>();
            var subject = new SqlMailQueue(repMock.Object);

            var last = new MAIL()
            {
                SentUTC = DateTime.UtcNow.AddHours(0) // buffer should be active
            };

            var sent = new List<Models.RenderedMail>();

            // ACT
            subject.Dequeue(m =>
            {
                sent.Add(m);
                return true;
            },
            () => batch,
            (uid) => last);

            // ASSERT
            Assert.AreEqual(1, sent.Count);

            var mail = sent.First();
            Assert.AreEqual("subject", mail.Subject);
        }

        [TestMethod]
        public void WithBuffer_ShouldWait()
        {
            // ARRANGE
            var batch = GetBatch();
            batch.Add(new MAIL()
            {
                UserID = "example-user",
                SenderEmailAddress = "sender@example.org",
                ReceiverEmailAddress = "receiver@example.org",
                EnqueuedUTC = DateTime.UtcNow,
                NextProcessingAttemptUTC = DateTime.UtcNow,
                Subject = "subject",
                Body = "body",
                SummaryHeader = "Welcome,",
                SummaryFooter = "Thanks",
                SummarySubject = "summary",
                SummaryBody = "summarybody",

                BufferHours = 4
            });

            var repMock = new Mock<IRepository<MAIL>>();
            var subject = new SqlMailQueue(repMock.Object);

            var last = new MAIL()
            {
                SentUTC = DateTime.UtcNow.AddHours(0) // buffer should be active
            };

            var sent = new List<Models.RenderedMail>();

            // ACT
            subject.Dequeue(m =>
            {
                sent.Add(m);
                return true;
            },
            () => batch,
            (uid) => last);

            // ASSERT
            Assert.AreEqual(0, sent.Count);
        }

        [TestMethod]
        public void WithBuffer_FlushIgnoresBuffer()
        {
            // ARRANGE
            var batch = GetBatch();
            batch.Add(new MAIL()
            {
                UserID = "example-user",
                SenderEmailAddress = "sender@example.org",
                ReceiverEmailAddress = "receiver@example.org",
                EnqueuedUTC = DateTime.UtcNow,
                NextProcessingAttemptUTC = DateTime.UtcNow,
                Subject = "subject",
                Body = "body",
                SummaryHeader = "Welcome,",
                SummaryFooter = "Thanks",
                SummarySubject = "summary",
                SummaryBody = "summarybody",

                BufferFlush = true,
                BufferHours = 4
            });

            var repMock = new Mock<IRepository<MAIL>>();
            var subject = new SqlMailQueue(repMock.Object);

            var last = new MAIL()
            {
                SentUTC = DateTime.UtcNow.AddHours(0) // buffer should be active
            };

            var sent = new List<Models.RenderedMail>();

            // ACT
            subject.Dequeue(m =>
            {
                sent.Add(m);
                return true;
            },
            () => batch,
            (uid) => last);

            // ASSERT
            Assert.AreEqual(1, sent.Count);

            var mail = sent.First();
            Assert.AreEqual("summary", mail.Subject);
            const string assembledBody =
            @"Welcome,
s0
s1
s2
s3
summarybody
Thanks
";
            Assert.AreEqual(assembledBody, mail.Body);

        }

        [TestMethod]
        public void WithBuffer_FlushWhenWindowIsOver()
        {
            // ARRANGE
            var batch = GetBatch();
            batch.Add(new MAIL()
            {
                UserID = "example-user",
                SenderEmailAddress = "sender@example.org",
                ReceiverEmailAddress = "receiver@example.org",
                EnqueuedUTC = DateTime.UtcNow,
                NextProcessingAttemptUTC = DateTime.UtcNow,
                Subject = "subject",
                Body = "body",
                SummaryHeader = "Welcome,",
                SummaryFooter = "Thanks",
                SummarySubject = "summary",
                SummaryBody = "summarybody",

                BufferHours = 4
            });

            var repMock = new Mock<IRepository<MAIL>>();
            var subject = new SqlMailQueue(repMock.Object);

            var last = new MAIL()
            {
                SentUTC = DateTime.UtcNow.AddHours(-5) // buffer should be active
            };

            var sent = new List<Models.RenderedMail>();

            // ACT
            subject.Dequeue(m =>
            {
                sent.Add(m);
                return true;
            },
            () => batch,
            (uid) => last);

            // ASSERT
            Assert.AreEqual(1, sent.Count);

            var mail = sent.First();
            Assert.AreEqual("summary", mail.Subject);
            const string assembledBody =
            @"Welcome,
s0
s1
s2
s3
summarybody
Thanks
";
            Assert.AreEqual(assembledBody, mail.Body);

        }

        [TestMethod]
        public void WithBuffer_FlushWhenWindowIsOver_WithHtml()
        {
            // ARRANGE
            var batch = new List<MAIL>();
            batch.Add(new MAIL()
            {
                UserID = "example-user",
                SenderEmailAddress = "sender@example.org",
                ReceiverEmailAddress = "receiver@example.org",
                EnqueuedUTC = DateTime.UtcNow,
                NextProcessingAttemptUTC = DateTime.UtcNow,
                Subject = "subject",
                SummarySubject = "summary",

                Body = "htmlbody",
                SummaryHeader = "htmlWelcome,",
                SummaryFooter = "htmlThanks",
                SummaryBody = "htmlsummarybody",

                TextBody = "textbody",
                SummaryTextHeader = "textWelcome,",
                SummaryTextFooter = "textThanks",
                SummaryTextBody = "textsummarybody",

                BufferHours = 4
            });

            var repMock = new Mock<IRepository<MAIL>>();
            var subject = new SqlMailQueue(repMock.Object);

            var last = new MAIL()
            {
                SentUTC = DateTime.UtcNow.AddHours(-5) // buffer should be active
            };

            var sent = new List<Models.RenderedMail>();

            // ACT
            subject.Dequeue(m =>
            {
                sent.Add(m);
                return true;
            },
            () => batch,
            (uid) => last);

            // ASSERT
            Assert.AreEqual(1, sent.Count);

            var mail = sent.First();
            Assert.AreEqual("summary", mail.Subject);
            const string assembledHtmlBody =
            @"htmlWelcome,
htmlsummarybody
htmlThanks
";
            Assert.AreEqual(assembledHtmlBody, mail.Body);

            const string assembledTextBody =
          @"textWelcome,
textsummarybody
textThanks
";
            Assert.AreEqual(assembledTextBody, mail.TextBody);

        }

        private static List<MAIL> GetBatch()
        {
            var batch = new List<MAIL>();

            for (int i = 4; i > 0; i--)
            {
                batch.Add(new MAIL()
                {
                    UserID = "example-user",
                    SenderEmailAddress = "sender@example.org",
                    ReceiverEmailAddress = "receiver@example.org",
                    Subject = "subject" + (4 - i).ToString(),
                    Body = "body" + (4 - i).ToString(),
                    SummaryHeader = "Welcome,",
                    SummaryFooter = "Thanks",
                    SummarySubject = "summary",
                    SummaryBody = "s" + (4 - i).ToString(),
                    EnqueuedUTC = DateTime.UtcNow.AddHours(-1 * i),
                    NextProcessingAttemptUTC = DateTime.UtcNow.AddHours(-1 * i)
                });
            }

            return batch;
        }
    }
}
