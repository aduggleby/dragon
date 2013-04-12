using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotificationTest
{
    [TestClass]
    public class FileSystemLocalizedDataSourceIntegrationTest
    {
        [TestMethod]
        public void GetContentShouldReturnContent()
        {
            var fileSystemLocalizedDataSource = new FileSystemLocalizedDataSource("../../res", "txt");
            var actual = fileSystemLocalizedDataSource.GetContent("send_message", "de-AT");
            Assert.AreEqual("test\r\nmessage\r\nde-AT", actual);
        }
    }
}
