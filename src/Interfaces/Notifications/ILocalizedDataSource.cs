using System;

namespace Dragon.Interfaces.Notifications
{
    public interface ILocalizedDataSource
    {
        String GetContent(String key, String languageCode);
    }
}
