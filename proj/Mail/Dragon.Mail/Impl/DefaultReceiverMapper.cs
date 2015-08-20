using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;

namespace Dragon.Mail.Impl
{
    public class DefaultReceiverMapper : IReceiverMapper
    {
        public void Map(dynamic receiver, Models.Mail mail)
        {
            var displayName = receiver.fullname;
            var email = receiver.email;

            mail.Receiver = new System.Net.Mail.MailAddress(email, displayName);
        }
    }
}
