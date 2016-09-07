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
            var displayName = (string)null;
            if (HasProperty(receiver, "fullname"))
            {
                displayName = receiver.fullname;
            }
            if (!HasProperty(receiver, "email"))
            {
                throw new Exception("The receiver object must have an email property denoting who to send the e-mail to.");
            }
            var email = receiver.email;

            mail.Receiver = new System.Net.Mail.MailAddress(email, displayName);
        }

        private bool HasProperty(dynamic receiver, string name)
        {
            if (receiver is ExpandoObject)
            {
                return ((IDictionary<String, object>)receiver).ContainsKey(name);
            }
            else
            {
                return receiver.GetType().GetProperty(name) != null;
            }
        }
    }
}
