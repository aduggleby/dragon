using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using Dapper;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotificationTest
{

    /// <summary>
    /// Needs a connection string, provided like in the Demo project.
    /// Expects tables to be empty and resets them, so use a test database.
    /// </summary>
    [TestClass]
    public class SqlNotificationStoreIT
    {
        private const string CONFIG_KEY_CONNECTION_STRING = "Dragon";
        private SqlNotificationStore _store;

        # region test data

        private readonly Guid _userID = Guid.NewGuid();
        private readonly INotification _notification = new Notification
        {
            ID = Guid.NewGuid(), 
            Subject = "subject", 
            LanguageCode = "de", 
            TypeKey = "hello", 
            Dispatched = true,
            Parameter = new Dictionary<string, string>
            {
                {"arg1", "arg"}
            }
        };
        private readonly INotification _notification2 = new Notification
        {
            Subject = "subject2",
            LanguageCode = "en",
            TypeKey = "hello2",
            Dispatched = false,
            Parameter = new Dictionary<string, string>
            {
                {"arg22", "arg2"}
            }
        };

        private readonly INotification _notification3 = new Notification
        {
            Subject = "subject3",
            LanguageCode = "en",
            TypeKey = "hello3",
            Dispatched = false,
            Parameter = new Dictionary<string, string>
            {
                {"arg33", "arg3"}
            }
        };

        # endregion

        [TestInitialize]
        public void Initialize()
        {
            _store = new SqlNotificationStore(GetConnectionString());
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                conn.Execute("DELETE FROM [Notification]", new {});
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void AddShouldPersistNotification()
        {
            _store.Add(_userID, _notification);

            Notification actual;
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var result = conn.Query<Notification>("SELECT [NotificationID], [TypeKey], [LanguageCode], [Subject], [Dispatched] FROM [Notification]", new { });
                var notifications = (result as IList<Notification> ?? result.ToList());
                Assert.AreEqual(1, notifications.Count());
                actual = notifications.First();
                actual.ID = conn.Query<Guid>("SELECT [NotificationID] FROM [Notification]", new {}).First();
                actual.Parameter = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(
                    conn.Query<String>("SELECT [Parameter] FROM [Notification]", new {}).First());
            }
            Assert.AreEqual(_notification, actual);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetAllShouldReturnAllNotificationsOfUser()
        {
            _store.Add(_userID, _notification);
            _store.Add(_userID, _notification2);

            var actual = _store.GetAll(_userID);
            var notifications = actual as IList<INotification> ?? actual.ToList();
            Assert.AreEqual(2, notifications.Count());
            Assert.IsTrue(notifications.Contains(_notification));
            Assert.IsTrue(notifications.Contains(_notification2));
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetAllUndispatchedShouldReturnOnlyUndispatchedNotificationsOfUser()
        {
            _store.Add(_userID, _notification);
            _store.Add(_userID, _notification2);

            var actual = _store.GetAllUndispatched(_userID);
            var notifications = actual as IList<INotification> ?? actual.ToList();
            Assert.AreEqual(1, notifications.Count());
            Assert.IsTrue(notifications.Contains(_notification2));
            Assert.IsFalse(notifications.Contains(_notification));
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void SetDispatchedShouldUpdateNotification()
        {
            _store.Add(_userID, _notification2);
            _store.SetDispatched(_notification2.ID);

            var actual = _store.GetAll(_userID);
            var notifications = actual as IList<INotification> ?? actual.ToList();
            Assert.AreEqual(1, notifications.Count());
            Assert.AreEqual(true, notifications.First().Dispatched);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void SetDispatchedAllShouldUpdateAllNotificationsOfUser()
        {
            _store.Add(_userID, _notification2);
            _store.Add(_userID, _notification3);
            _store.SetAllDispatched(_userID);

            var actual = _store.GetAll(_userID);
            var notifications = actual as IList<INotification> ?? actual.ToList();
            Assert.AreEqual(2, notifications.Count());
            foreach (var notification in notifications)
            {
                Assert.AreEqual(true, notification.Dispatched);                
            }
        }

        # region helpers

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[CONFIG_KEY_CONNECTION_STRING].ToString();
        }

        # endregion
    }
}
