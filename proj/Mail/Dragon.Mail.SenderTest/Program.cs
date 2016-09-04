using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Data;
using Dragon.Mail.Impl;
using Dragon.Mail.SqlQueue;

namespace Dragon.Mail.SenderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var md = new TableMetadata();

            //MetadataHelper.MetadataForClass(typeof(Dragon.Mail.SqlQueue.Mail), ref md);
            //var s = TSQLGenerator.BuildCreate(md);

            var queue = new InMemoryMailQueue();
            var senderx = new MailSenderService(queue);

            var generator = new MailGeneratorService(queue, mailSenderService: senderx, async: false);


            var loader = new FileFolderTemplateRepository(@"..\..\templates");

            loader.EnumerateTemplates(generator.Register);

            dynamic recipient = new ExpandoObject();
            recipient.email = "bob@example.org";
            recipient.fullname = "Bob";
            recipient.userid = "baxtor";
            
            dynamic data = new ExpandoObject();
            data.link = "http://www.google.com";
            data.name = "Google";

            generator.Send(recipient, "Welcome", data);

            //while (senderx.ProcessNext())
            //{
            //}
        }
    }
}
