using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Interfaces;
using Dragon.Interfaces.ActivityCenter;
using Dragon.Interfaces.Core;

namespace Dragon.ActivityCenter.Notification
{
    public class EmailNotificationDispatcher : INotificationDispatcher
    {
        private readonly IEmailService m_emailService;
        private readonly IEmailTemplateService m_templateService;
        private readonly IEmailProfileService m_profileService;

        public EmailNotificationDispatcher(
            IEmailService emailService,
            IEmailTemplateService templateService,
            IEmailProfileService profileService)
        {
            m_emailService = emailService;
            m_templateService = templateService;
            m_profileService = profileService;
        }

        public void Notify(IActivity activity, INotifiable notifiable)
        {
            if (!(activity is IEmailNotificationActivity)) return;

            var activitiesList = m_profileService
                .AppendAndFlushIfNecessary(notifiable, activity);

            // if empty, nothing to flush, but activity is stored
            if (activitiesList==null) return;
            var activities = activitiesList.ToList();
            if (activities.Any()) 
            {
                var profile = m_profileService.GetProfile(notifiable);

                if (string.IsNullOrWhiteSpace(profile.PrimaryEmailAddress)) return;

                var activityTypes = activities.GroupBy(x => x.GetType().TypeHandle).ToList();

                var subject = "";
                var body = new StringBuilder();
                var atLeastOneHtml = false;
                var multipleTypes = (activityTypes.Count() > 1) ;
                foreach (var activityType in activityTypes)
                {
                    var onlyText = new string[] {"text"};
                    var tryHtmlAndText = new string[] {"html", "text"};


                    var template = m_templateService.Generate(
                        activityType,
                        profile.WantsHtml ? tryHtmlAndText : onlyText,
                        notifiable);
                    var thisHtml = template.Subtype == "html";
                    subject = template.Subject;
                    if (multipleTypes)
                    {
                        body.AppendLine(thisHtml?PWrap(subject):subject);
                    }

                    body.AppendLine(thisHtml ? PWrap(template.Body) : template.Body);
                    atLeastOneHtml = atLeastOneHtml || thisHtml;   
                }

                if (multipleTypes)
                    subject = "Updates"; // TODO I18N

                m_emailService.Send(
                    profile.PrimaryEmailAddress,
                    subject,
                    body.ToString(),
                    atLeastOneHtml
                    );
            }
        }

        private static string PWrap(string subject)
        {
            return string.Format("<p>{0}</p>",subject);
        }
    }
}
