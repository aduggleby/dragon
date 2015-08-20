using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface IReceiverMapper
    {
        void Map(dynamic receiver, Models.Mail mail);
    }
}
