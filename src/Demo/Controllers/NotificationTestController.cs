using System.Collections.Generic;
using System.Web.Mvc;
using Dragon.Interfaces;
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
        public ActionResult EMailTest(string email, string subject, string key)
        {
            // TODO: inject this stuff
            var notifiable = new EmailNotifiable { PrimaryEmailAddress = email, UseHTMLEmail = false };
            var notification = new Notification
            {
                LanguageCode = "de",
                Parameter = new Dictionary<string, string> {{"name", email}},
                Subject = subject,
                TypeKey = key,
            };
            var dispatcher = new EmailNotificationDispatcher(
                new NetEmailService {Configuration = ObjectFactory.GetInstance<IConfiguration>()}, new StringTemplateTemplateService(), new FileSystemLocalizedDataSource(HttpContext.Server.MapPath("~/Resources/templates"), "txt")); // TODO: move to config
            dispatcher.Dispatch(notifiable, notification);
            return View();
        }

    }
}
