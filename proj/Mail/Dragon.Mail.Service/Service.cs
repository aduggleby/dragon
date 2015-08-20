using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Impl;
using Topshelf;

namespace Dragon.Mail.Service
{
    public class Service
    {
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<MailSenderService>(s =>
                {
                    s.ConstructUsing(name => new MailSenderService(new FileFolderMailQueue()));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Windows Service for Dragon.Mail async and buffered mail sending");
                x.SetDisplayName("Dragon Mail Service");
                x.SetServiceName("Dragon.Mail.Service");
            });
        }
    }
}
