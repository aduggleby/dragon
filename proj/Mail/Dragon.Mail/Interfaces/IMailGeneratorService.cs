using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Models;

namespace Dragon.Mail.Interfaces
{
    public interface IMailGeneratorService
    {
        void Register(Template template);

        void Send(dynamic receiver, string templateKey, dynamic data, CultureInfo language = null);
    }
}
