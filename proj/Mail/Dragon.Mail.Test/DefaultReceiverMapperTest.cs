using System;
using System.Dynamic;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class DefaultReceiverMapperTest
    {
        [TestMethod]
        public void Basic()
        {
            // ARRANGE
            var subject = new DefaultReceiverMapper();
            var mail = new Models.Mail();
            dynamic receiver = new ExpandoObject();
            receiver.fullname = "Markus Boxington";
            receiver.email = "markus@boxington.example.org";

            // ACT
            subject.Map(receiver, mail);

            // ASSERT
            Assert.AreEqual("markus@boxington.example.org", mail.Receiver.Address);
            Assert.AreEqual("Markus Boxington", mail.Receiver.DisplayName);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DifferentCase_ThrowsException()
        {
            // ARRANGE
            var subject = new DefaultReceiverMapper();
            var mail = new Models.Mail();
            dynamic receiver = new ExpandoObject();
            receiver.FuLlName = "Markus Boxington";
            receiver.eMAIl = "markus@boxington.example.org";

            // ACT
            subject.Map(receiver, mail);

            // ASSERT
            Assert.AreEqual("markus@boxington.example.org", mail.Receiver.Address);
            Assert.AreEqual("Markus Boxington", mail.Receiver.DisplayName);

        }

    }
}
