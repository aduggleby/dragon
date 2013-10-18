using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Dragon.Interfaces.ActivityCenter;

namespace Dragon.ActivityCenter
{
    public class ActivityDispatcher : IActivityDispatcher
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLogger();

        private readonly IActivityService m_activityService;
        private readonly List<IActivityMultiplexer> m_multiplexer;
        private readonly List<INotificationDispatcher> m_notificationDispatchers;

        public ActivityDispatcher(
            IActivityService activityService,
            List<IActivityMultiplexer> multiplexer, 
            List<INotificationDispatcher> notificationDispatchers)
        {
            m_activityService = activityService;
            m_multiplexer = multiplexer;
            m_notificationDispatchers = notificationDispatchers;

            if (m_multiplexer == null || !m_multiplexer.Any())
                s_log.Warn("No multiplexer defined. No notifications will be generated.");

            if (m_notificationDispatchers == null || !m_notificationDispatchers.Any())
                s_log.Warn("No notification dispatchers defined. No notifications will be dispatched.");
        }

        public void Dispatch(IActivity activity)
        {
            s_log.Info(x => x("Dispatching {0}", activity.ToString()));
            m_activityService.Save(activity);

            foreach (var notifiable in m_multiplexer.SelectMany(x => x.Multiplex(activity)))
            {
                m_activityService.SaveNotification(activity, notifiable);
                s_log.Debug(x=>x("Dispatching {0} to {1}", activity.ToString(), notifiable.ToString()));
                m_notificationDispatchers.ForEach(x =>
                {
                    x.Notify(activity, notifiable);
                });
            }
        }
    }
}
