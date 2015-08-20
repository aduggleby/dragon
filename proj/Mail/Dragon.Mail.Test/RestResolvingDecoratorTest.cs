using System;
using System.Dynamic;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class RestResolvingDecoratorTest
    {
        [TestMethod]
        public void ResolvesWithCacheCorrectly()
        {
            // ARRANGE
            var httpmock = new Mock<IHttpClient>();
            httpmock.Setup(m => m.GetAsync(new Uri("http://example.org/url1")))
                .ReturnsAsync(new
                {
                    test1 = "val1",
                    test2 = "val2"
                });
            httpmock.Setup(m => m.GetAsync(new Uri("http://example.org/url2")))
                .ReturnsAsync(new
                {
                    t = new { a = "b", c = "d" },
                });

            var dataStoreMock = new Mock<IDataStore>();
            dataStoreMock.Setup(m => m.Get(new Uri("http://example.org/url1")))
                .Returns(new
                {
                    test1 = "val1cached",
                    test2 = "val2"
                });
           
            var subject = new RestResolvingDecorator(httpmock.Object, dataStoreMock.Object);

            // ACT
            var actual = subject.Decorate(new
            {
                person = new Uri("http://example.org/url1"),
                team = new Uri("http://example.org/url2")
            });

            // ASSERT
            Assert.AreEqual("val1cached", actual.person.test1);
            Assert.AreEqual("val2", actual.person.test2);

            Assert.AreEqual("b", actual.team.t.a);

            dataStoreMock.Verify(m => m.Set(
                   new Uri("http://example.org/url1"),
                   It.IsAny<object>()),
               Times.Never);

            dataStoreMock.Verify(m => m.Set(
                    new Uri("http://example.org/url2"),
                    It.IsAny<object>()),
                Times.Once);

        }

        [TestMethod]
        public void DoesNotTouchNonUriProperties()
        {

            // ARRANGE
            var httpmock = new Mock<IHttpClient>();
            
            var subject = new RestResolvingDecorator(httpmock.Object);

            dynamic user = new ExpandoObject();
            user.email = "bob@example.org";
            user.name = "Bob";


            // ACT
            var actual = subject.Decorate(user);

            // ASSERT
            Assert.AreEqual("bob@example.org", actual.email);
            Assert.AreEqual("Bob", actual.name);
        }
    }
}
