using System;

namespace Dragon.Context
{
    public interface IContext
    {
        Guid CurrentUserID { get; }
        void Load();
        void Save(Guid currentUserId);
        bool IsAuthenticated();
    }
}
