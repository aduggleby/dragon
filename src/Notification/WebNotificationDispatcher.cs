using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Dragon.Interfaces.Notifications;
using Microsoft.AspNet.SignalR;

namespace Dragon.Notification
{
    // TODO: test
    public class WebNotificationDispatcher : INotificationDispatcher<IWebNotifiable>
    {
        private readonly ITemplateService _templateService;
        private readonly ILocalizedDataSource _dataSource;
        private readonly INotificationStore _notificationStore;
        private static Boolean _isInitialized;
        private static IHubContext _context;

        public class NotificationHub : Hub
        {
            public static WebNotificationDispatcher Dispatcher { get; set; }

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
            /// Sends all notifications that a user has not yet acknowledged to the specific client.
            /// </summary>
            /// <param name="userID">The user to whom the undispatched notification belongs</param>
            /// <returns>A JSON serialized list of notifications</returns>
            public Task GetUndispatchedNotifications(String userID)
            {
                return Task.Factory.StartNew(() => Dispatcher.DispatchAllUndispatched(new Guid(userID), Context.ConnectionId));
            }

            /// <summary>
            /// Needs to be called by the client once per user to acknowledge the reception of a notification.
            /// </summary>
            /// <param name="userID">The user</param>
            /// <param name="notificationID">The notification to acknowledge</param>
            public Task SetMessageRead(String userID, String notificationID)
            {
                return Task.Factory.StartNew(() => Dispatcher.SetNotificationDispatched(userID, notificationID));
            }

            /// <summary>
            /// Can be called by the client to acknowledge the reception of all notifications of a specific user.
            /// </summary>
            /// <param name="userID">The user</param>
            public Task SetAllMessagesRead(String userID)
            {
                return Task.Factory.StartNew(() => Dispatcher.SetNotificationsDispatched(userID));
            }
        }

        public WebNotificationDispatcher(ITemplateService templateService, ILocalizedDataSource dataSource, INotificationStore notificationStore)
        {
            _templateService = templateService;
            _dataSource = dataSource;
            _notificationStore = notificationStore;
        }

        public void Dispatch(IWebNotifiable notifiable, INotification notification)
        {
            if (!_isInitialized)
            {
                throw new WebNotificationException(
                    "Init() needs to be called in Application_Start() of Global.asax before dispatching notifications!");
            }

            _context.Clients.Group(notifiable.UserID.ToString()).addNotification(BuildNotification(notification));
        }

        public void DispatchAllUndispatched(Guid userID, string connectionID)
        {
            var notifications = _notificationStore.GetAllUndispatched(userID).Select(BuildNotification).ToList();
            _context.Clients.Client(connectionID).addNotifications(new JavaScriptSerializer().Serialize(notifications));
        }

        public void SetNotificationDispatched(string userID, string notificationID)
        {
            _notificationStore.SetDispatched(new Guid(notificationID));
            _context.Clients.Group(userID).notifyNotificationRead(notificationID);
        }

        private void SetNotificationsDispatched(string userID)
        {
            _notificationStore.SetAllDispatched(new Guid(userID));
            _context.Clients.Group(userID).notifyAllNotificationsRead();
        }

        /// <summary>
        /// Needs to be called in Application_Start() of Global.asax, see http://www.asp.net/signalr/overview/.
        /// </summary>
        public static void Init()
        {
            RouteTable.Routes.MapHubs();
            _context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            _isInitialized = true;
        }

        # region helpers

        private string BuildNotification(INotification notification)
        {
            var bodyTemplate = _dataSource.GetContent(notification.TypeKey, notification.LanguageCode);
            return _templateService.Parse(bodyTemplate, notification.Parameter);
        }

        # endregion
    }

}
