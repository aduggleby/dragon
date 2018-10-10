using System;
using Dragon.Context.Interfaces;

namespace Dragon.Context.Users
{
    public class User : IUser
    {
        public Guid RegistrationID { get; private set; }
        public Guid UserID { get; set; }
        public string Service { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }

        public User()
        {
            RegistrationID = Guid.NewGuid();
        }
    }
}
