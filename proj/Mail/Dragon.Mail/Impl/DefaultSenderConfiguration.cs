using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Utils;

namespace Dragon.Mail.Impl
{
    public class DefaultSenderConfiguration : ISenderConfiguration
    {
        public const string APP_KEY_FROM_ADDRESS = "Dragon.Mail.Sender.Address";
        public const string APP_KEY_FROM_NAME = "Dragon.Mail.Sender.Name";
        private readonly string m_address;
        private readonly string m_name;

        public DefaultSenderConfiguration(IConfiguration configuration)
        {
            m_address = configuration.GetValue(APP_KEY_FROM_ADDRESS);
            m_name = configuration.GetValue(APP_KEY_FROM_NAME);

            if (string.IsNullOrWhiteSpace(m_address))
            {
                throw new ConfigurationMissingException(APP_KEY_FROM_ADDRESS);
            }
        }

        public void Configure(Models.Mail mail)
        {
            if (string.IsNullOrWhiteSpace(m_name))
            {
                mail.Sender = new System.Net.Mail.MailAddress(m_address);
            }
            else
            {
                mail.Sender = new System.Net.Mail.MailAddress(m_address, m_name);
            }
        }
    }
}
