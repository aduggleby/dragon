using System;
using System.Collections.Specialized;
using Dragon.Interfaces;
using Moq;

namespace FilesTest
{
    public class TestHelper
    {
        public static Mock<IConfiguration> CreateConfigurationMock(NameValueCollection appSettings)
        {
            var configuration = new Mock<IConfiguration>();

            foreach (var setting in appSettings.AllKeys)
            {
                var settingTmp = setting;
                configuration.Setup(x => x.GetValue(settingTmp, It.IsAny<String>())).Returns(appSettings.Get(setting));
            }
            return configuration;
        }
    }
}
