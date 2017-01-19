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
            //FolderTest();
            //ResourceFileTest();
            ResourceFileTestInGerman();
            //while (senderx.ProcessNext())
            //{
            //}
        }

        static void FolderTest()
        {

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
        }

        static void ResourceFileTest()
        {
            var queue = new InMemoryMailQueue();

            var senderx = new MailSenderService(queue);

            var generator = new MailGeneratorService(queue, mailSenderService: senderx, async: false);

            var loader = new ResourceFileTemplateRepository(new DefaultResourceManagerAdapter(Templates.ResourceManager));
            loader.EnumerateTemplates(generator.Register);

            dynamic recipient = new ExpandoObject();
            recipient.email = "bob@example.org";
            recipient.fullname = "Bob";
            recipient.userid = "baxtor";

            dynamic data = new ExpandoObject();
            data.link = "http://www.google.com";
            data.name = "Google";

            generator.Send(recipient, "Welcome", data);
        }


        static void ResourceFileTestInGerman()
        {
            var queue = new InMemoryMailQueue();

            var senderx = new MailSenderService(queue);

            var generator = new MailGeneratorService(queue, mailSenderService: senderx, async: false);

            var loader = new ResourceFileTemplateRepository(new DefaultResourceManagerAdapter(Templates.ResourceManager));
            loader.EnumerateTemplates(generator.Register);

            dynamic recipient = new ExpandoObject();
            recipient.email = "bob@example.org";
            recipient.fullname = "Bob";
            recipient.userid = "baxtor";

            dynamic data = new ExpandoObject();
            data.link = "http://www.google.com";
            data.name = "Google";

            generator.Send(recipient, "Welcome", data, language: CultureInfo.CreateSpecificCulture("de-AT"));
        }
    }
}
