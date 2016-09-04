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


            var loader = new FileFolderTemplateRepository(@"G:\WhatAVenture\Design\Tool\UX\System Mails\templates");

            loader.EnumerateTemplates(generator.Register);

            dynamic recipient = new ExpandoObject();
            recipient.email = "bob@example.org";
            recipient.fullname = "Bob";
            recipient.userid = "baxtor";

            dynamic sender = new ExpandoObject();
            sender.email = "bob@example.org";
            sender.fullname = "Alice";
            sender.userid = "alicex";


            dynamic conversation = new ExpandoObject();
            conversation.url = "http://www.google.com";
            conversation.subject = "My Topic";
            conversation.message =
                "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.!";
            conversation.recipient = recipient;
            conversation.sender = sender;


            dynamic data = new ExpandoObject();
            data.conversation = conversation;

            generator.Send(recipient, "ConversationMessageNotification", data);

            //while (senderx.ProcessNext())
            //{
            //}
        }
    }
}
