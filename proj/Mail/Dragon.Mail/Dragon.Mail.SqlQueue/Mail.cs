using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Data.Attributes;

namespace Dragon.Mail.SqlQueue
{
    public class Mail
    {
        [Key]
        public Guid MailID { get; set; }

        public bool Sent { get; set; }

        public bool Error { get; set; }

        [Length("400")]
        public string SenderDisplayName { get; set; }

        [Length("400")]
        public string SenderEmailAddress { get; set; }

        [Length("400")]
        public string ReceiverDisplayName { get; set; }

        [Length("400")]
        public string ReceiverEmailAddress { get; set; }

        [Length("400")]
        public string Subject { get; set; }

        [Length("MAX")]
        public string Body { get; set; }

        [Length("MAX")]
        public string TextBody { get; set; }

        [Length("400")]
        public string SummarySubject { get; set; }

        [Length("MAX")]
        public string SummaryHeader { get; set; }

        [Length("MAX")]
        public string SummaryBody { get; set; }

        [Length("MAX")]
        public string SummaryFooter { get; set; }

        [Length("MAX")]
        public string SummaryTextHeader { get; set; }

        [Length("MAX")]
        public string SummaryTextBody { get; set; }

        [Length("MAX")]
        public string SummaryTextFooter { get; set; }


        public DateTime EnqueuedUTC { get; set; }
        public DateTime NextProcessingAttemptUTC { get; set; }
        public DateTime? LastProcessedUTC { get; set; }
        public DateTime? SentUTC { get; set; }

        public string UserID { get; set; }
        public int BufferHours { get; set; }
        public bool BufferIgnored { get; set; }
        public bool BufferFlush { get; set; }
    }
}
