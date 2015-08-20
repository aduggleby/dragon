using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Utils;
using Newtonsoft.Json;

namespace Dragon.Mail.Impl
{
    public class InMemoryMailQueue : IMailQueue, IDisposable
    {
        private Queue<Models.Mail> m_mailQueue = new Queue<Models.Mail>();
 
        public InMemoryMailQueue()
        {
       
        }

        public void Enqueue(Models.Mail mail, dynamic additionalParameters)
        {
            m_mailQueue.Enqueue(mail);
        }

        public bool Dequeue(Func<Models.RenderedMail, bool> processor)
        {
            var newestItem = m_mailQueue.Dequeue();

            if (newestItem != null)
            {

                var rendered = new RenderedMail();
                rendered.Receiver = newestItem.Receiver;
                rendered.Sender = newestItem.Sender;
                rendered.Subject = newestItem.Subject;
                rendered.Body = newestItem.Body;
                rendered.TextBody = newestItem.TextBody;

                return processor(rendered);
            }

            return false;
          
        }

        public void Dispose()
        {

        }
    }
}
