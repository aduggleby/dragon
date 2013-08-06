using System;
using System.Threading.Tasks;
using System.Web.Routing;
using Dragon.Interfaces.Notifications;
using Microsoft.AspNet.SignalR;

namespace Dragon.Notification
{
    // TODO: test
    public class WebNotificationDispatcher : INotificationDispatcher<IWebNotifiable>
    {
        private readonly ITemplateService _templateService;
        private readonly ILocalizedDataSource _dataSource;
        private static Boolean _isInitialized;

        public class NotificationHub : Hub
        {
            public static INotificationStore NotificationStore { get; set; }

            public void Send(string message)
            {
                // TODO: allow client to communicate to the server?
            }

            /// <summary>
            /// This method needs to be called in order to be able to target user specific notifications.
            /// </summary>
            /// <param name="userID">The userID is passed from the client to avoid dependency to DragonContext.</param>
            public Task Login(String userID)
            {
                return Groups.Add(Context.ConnectionId, userID);
            }

            /// <summary>
            /// This method can be called to unsubscribe a client of an user.
            /// Other connections of the same user will stay active.
            /// </summary>
            /// <param name="userID">The user which is unsubscribed</param>
            public Task Logout(String userID)
            {
                return Groups.Remove(Context.ConnectionId, userID);
            }

            /// <summary>
            /// Returns all notifications that a user has not yet acknowledged.
            /// </summary>
            /// <param name="userID">The user to whom the undispatched notification belongs</param>
            public Task GetUndispatchedNotifications(String userID)
            {
                return new Task(() => NotificationStore.GetAllUndispatched(new Guid(userID)));
            }

            /// <summary>
            /// Needs to be called by the client to acknowledge the reception of a notification.
            /// </summary>
            /// <param name="userID">The user</param>
            /// <param name="notificationID">The notification to acknowledge</param>
            public Task SetMessageRead(String userID, String notificationID)
            {
                return new Task(() =>
                {
                    NotificationStore.SetDispatched(new Guid(notificationID));
                    Clients.Group(userID).notifyNotificationRead(notificationID);
                });
            }

            /// <summary>
            /// Can be called by the client to acknowledge the reception of all notifications of a specific user.
            /// </summary>
            /// <param name="userID">The user</param>
            public Task SetMessagesRead(String userID)
            {
                return new Task(() =>
                {
                    NotificationStore.SetAllDispatched(new Guid(userID));
                    Clients.Group(userID).notifyAllNotificationsRead();
                });
            }
        }

        public WebNotificationDispatcher(ITemplateService templateService, ILocalizedDataSource dataSource, INotificationStore notificationStore)
        {
            _templateService = templateService;
            _dataSource = dataSource;
            NotificationHub.NotificationStore = notificationStore;
        }

        public void Dispatch(IWebNotifiable notifiable, INotification notification)
        {
            if (!_isInitialized)
            {
                throw new WebNotificationException(
                    "Init() needs to be called in Application_Start() of Global.asax before dispatching notifications!");
            }
            var bodyTemplate = _dataSource.GetContent(notification.TypeKey, notification.LanguageCode);
            var body = _templateService.Parse(bodyTemplate, notification.Parameter);
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Group(notifiable.UserID.ToString()).addMessage(body);
        }

        /// <summary>
        /// Needs to be called in Application_Start() of Global.asax, see http://www.asp.net/signalr/overview/.
        /// </summary>
        public static void Init()
        {
            RouteTable.Routes.MapHubs();
            _isInitialized = true;
        }
    }

}
