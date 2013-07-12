using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotificationTest
{
    [TestClass]
    public class EmailEqComparerTest
    {
        [TestMethod]
        public void EqualsSameEmailAddressesShouldReturnTrue()
        {
            var comparer = new EmailBatchNotificationDispatcher.EmailEqComparer();
            Assert.IsTrue(comparer.Equals(GetNotifiable1(), GetNotifiable2()));
        }

        [TestMethod]
        public void GetHashCodeSameEmailsShouldBeEqual()
        {
            var comparer = new EmailBatchNotificationDispatcher.EmailEqComparer();
            Assert.AreEqual(comparer.GetHashCode(GetNotifiable1()), comparer.GetHashCode(GetNotifiable2()));
        }

        [TestMethod]
        public void EqualsDifferentEmailAddressesShouldReturnFalse()
        {
            var comparer = new EmailBatchNotificationDispatcher.EmailEqComparer();
            Assert.IsFalse(comparer.Equals(GetNotifiable1(), GetNotifiable3()));
        }

        [TestMethod]
        public void GetHashCodeDifferentEmailsShouldNotBeEqual()
        {
            var comparer = new EmailBatchNotificationDispatcher.EmailEqComparer();
            Assert.AreNotEqual(comparer.GetHashCode(GetNotifiable1()), comparer.GetHashCode(GetNotifiable3()));
        }

        private IEmailNotifiable GetNotifiable2()
        {
            return new EmailNotifiable
            {
                PrimaryEmailAddress = "user@domain.tld",
                UseHTMLEmail = false
            };
        }

        private IEmailNotifiable GetNotifiable1()
        {
            return new EmailNotifiable
            {
                PrimaryEmailAddress = "user@domain.tld",
                UseHTMLEmail = true
            };
        }

        private IEmailNotifiable GetNotifiable3()
        {
            return new EmailNotifiable
            {
                PrimaryEmailAddress = "user2@domain.tld",
                UseHTMLEmail = true
            };
        }
    }
}
