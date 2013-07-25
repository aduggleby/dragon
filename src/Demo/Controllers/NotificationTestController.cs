using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Dragon.Interfaces;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using StructureMap;

namespace Demo.Controllers
{
    public class NotificationTestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string subject, string key)
        {
            var action = Request.Form["action"];
            switch (action)
            {
                case "EnqueueMail":
                    return EnqueueMail(email, subject, key);
                case "BatchSendMails":
                    return BatchSendMails(email, subject);
                case "SendMail":
                    return SendMail(email, subject, key);
            }
            throw new HttpException(400, "Action not found");
        }

        private ActionResult BatchSendMails(string email, string subject)
        {
            var notifiable = CreateNotifiable(email);
            var dispatcher = CreateOrGetBatchEmailNotificationDispatcher();
            dispatcher.DispatchAll(notifiable, subject);
            return Json(""); // TODO: return PartialView("NotificationResultPartial");
        }

        private ActionResult EnqueueMail(string email, string subject, string key)
        {
            var notifiable = CreateNotifiable(email);
            var notification = CreateNotification(email, subject, key);
            var dispatcher = CreateOrGetBatchEmailNotificationDispatcher();
            dispatcher.Dispatch(notifiable, notification);
            return Json(""); // TODO: return PartialView("NotificationResultPartial");
         }

        private IBatchNotificationDispatcher<IEmailNotifiable> CreateOrGetBatchEmailNotificationDispatcher()
        {
            if (Session["EmailBatchNotificationDispatcher"] == null)
            {
                Session["EmailBatchNotificationDispatcher"] = new EmailBatchNotificationDispatcher(
                    new NetEmailService {Configuration = ObjectFactory.GetInstance<IConfiguration>()},
                    new StringTemplateTemplateService(),
                    new FileSystemLocalizedDataSource(HttpContext.Server.MapPath("~/Resources/templates"), "txt"));
            }
           return (IBatchNotificationDispatcher<IEmailNotifiable>) Session["EmailBatchNotificationDispatcher"];
        }

        private ActionResult SendMail(string email, string subject, string key)
        {
            var notifiable = CreateNotifiable(email);
            var notification = CreateNotification(email, subject, key);
            var dispatcher = CreateEmailNotificationDispatcher();
            dispatcher.Dispatch(notifiable, notification);

            var webNotifiable = new WebNotifiable();

            var webDispatcher = new WebNotificationDispatcher(
                new StringTemplateTemplateService(),
                new FileSystemLocalizedDataSource(HttpContext.Server.MapPath("~/Resources/templates"), "txt"));
            webDispatcher.Dispatch(webNotifiable, notification);

            return Json(""); // TODO: return PartialView("NotificationResultPartial");
        }

        private static EmailNotifiable CreateNotifiable(string email)
        {
            var notifiable = new EmailNotifiable {PrimaryEmailAddress = email, UseHTMLEmail = false};
            return notifiable;
        }

        private static Notification CreateNotification(string email, string subject, string key)
        {
            var notification = new Notification
            {
                LanguageCode = "de",
                Parameter = new Dictionary<string, string> {{"name", email}},
                Subject = subject,
                TypeKey = key,
            };
            return notification;
        }

        private EmailNotificationDispatcher CreateEmailNotificationDispatcher()
        {
            var dispatcher = new EmailNotificationDispatcher(
                new NetEmailService {Configuration = ObjectFactory.GetInstance<IConfiguration>()},
                new StringTemplateTemplateService(),
                new FileSystemLocalizedDataSource(HttpContext.Server.MapPath("~/Resources/templates"), "txt"));
            // TODO: move to config
            return dispatcher;
        }
    }
}
