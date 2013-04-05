using System.Collections.Generic;
using System.Linq.Expressions;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationTest
{
    [TestClass]
    public class NetEmailServiceTest
    {
        private const string SMTP_SERVER = "localhost";
        private const int SMTP_PORT = 25;

        [TestMethod]
        public void DispatchShouldInvokeEmailService()
        {
            var netEmailService = new NetEmailService();
            Assert.Fail();
        }
    }
}
