using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dragon.Mail.Models
{
    public class RenderedMail
    {
        public MailAddress Sender { get; set; }

        public MailAddress Receiver { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string TextBody { get; set; }

    }
}
