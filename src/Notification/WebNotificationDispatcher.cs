using System;
using System.Threading.Tasks;
using System.Web.Routing;
using Dragon.Interfaces.Notifications;
using Microsoft.AspNet.SignalR;

namespace Dragon.Notification
{
    // TODO: test and select proper data source
    // TODO: verify that route has been set, else throw exception
    public class WebNotificationDispatcher : INotificationDispatcher<IWebNotifiable>
    {
        private readonly ITemplateService _templateService;
        private readonly ILocalizedDataSource _dataSource;

        public class NotificationHub : Hub
        {
            public void Send(string message)
            {
                // TODO: add listener?
            }

            public Task Login(String userId)
            {
                return Groups.Add(Context.ConnectionId, userId);
            }

            public Task Logout(String userId)
            {
                return Groups.Remove(Context.ConnectionId, userId);
            }
        }

        public WebNotificationDispatcher(ITemplateService templateService, ILocalizedDataSource dataSource)
        {
            _templateService = templateService;
            _dataSource = dataSource;
        }

        public void Dispatch(IWebNotifiable notifiable, INotification notification)
        {
            var bodyTemplate = _dataSource.GetContent(notification.TypeKey, notification.LanguageCode);
            var body = _templateService.Parse(bodyTemplate, notification.Parameter);
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Group(notifiable.UserID.ToString()).addMessage(body);
        }

        public static void Init()
        {
            RouteTable.Routes.MapHubs();
        }
    }

}
