using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dragon.Mail.Models
{
    public class Mail
    {
        [JsonConverter(typeof(MailAddressSerializerConverter))]
        public MailAddress Sender { get; set; }

        [JsonConverter(typeof(MailAddressSerializerConverter))]
        public MailAddress Receiver { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        public string TextBody { get; set; }

        public string SummarySubject { get; set; }

        public string SummaryHeader { get; set; }
        public string SummaryBody { get; set; }
        public string SummaryFooter { get; set; }

        public string SummaryTextHeader { get; set; }
        public string SummaryTextBody { get; set; }
        public string SummaryTextFooter { get; set; }

    }
}
