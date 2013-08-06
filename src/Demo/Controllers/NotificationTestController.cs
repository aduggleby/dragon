using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Dragon.Context;
using Dragon.Core.Sql;
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
            switch (Request.Form["action"])
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
            return Json("Batch sent.");
        }

        private ActionResult EnqueueMail(string email, string subject, string key)
        {
            var notifiable = CreateNotifiable(email);
            var notification = CreateNotification(email, subject, key);
            var dispatcher = CreateOrGetBatchEmailNotificationDispatcher();
            dispatcher.Dispatch(notifiable, notification);
            return Json("Enqueued.");
        }

        private ActionResult SendMail(string email, string subject, string key)
        {
            var notifiable = CreateNotifiable(email);
            var notification = CreateNotification(email, subject, key);
            var dispatcher = CreateEmailNotificationDispatcher();
            dispatcher.Dispatch(notifiable, notification);

            var webNotifiable = new WebNotifiable
            {
                UserID = DragonContext.Current.CurrentUserID
            };

            var webDispatcher = new WebNotificationDispatcher(CreateTemplateService(), CreateDataSource(), CreateNotificationStore());
            webDispatcher.Dispatch(webNotifiable, notification);

            return Json("Sent.");
        }

        private INotificationStore CreateNotificationStore()
        {
            return new SqlNotificationStore(StandardSqlStore.ConnectionString);
        }

        private IBatchNotificationDispatcher<IEmailNotifiable> CreateOrGetBatchEmailNotificationDispatcher()
        {
            if (Session["EmailBatchNotificationDispatcher"] == null)
            {
                Session["EmailBatchNotificationDispatcher"] = new EmailBatchNotificationDispatcher(
                    new NetEmailService {Configuration = ObjectFactory.GetInstance<IConfiguration>()},
                    CreateTemplateService(), CreateDataSource());
            }
            return (IBatchNotificationDispatcher<IEmailNotifiable>) Session["EmailBatchNotificationDispatcher"];
        }

        private StringTemplateTemplateService CreateTemplateService()
        {
            return new StringTemplateTemplateService();
        }

        private FileSystemLocalizedDataSource CreateDataSource()
        {
            return new FileSystemLocalizedDataSource(HttpContext.Server.MapPath("~/Resources/templates"), "txt");
        }

        private EmailNotifiable CreateNotifiable(string email)
        {
            var notifiable = new EmailNotifiable {PrimaryEmailAddress = email, UseHTMLEmail = false};
            return notifiable;
        }

        private Notification CreateNotification(string email, string subject, string key)
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
                CreateTemplateService(), CreateDataSource());
            // TODO: move to config
            return dispatcher;
        }
    }
}