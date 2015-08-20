using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface IMailQueue
    {
        void Enqueue(Models.Mail mail, dynamic additionalParameters);

        bool Dequeue(Func<Models.RenderedMail, bool> processor);
    }
}
