using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Dragon.SecurityServer.AccountSTS.Helpers
{
    public class UserModel
    {
        protected bool Equals(UserModel other)
        {
            return UserID.Equals(other.UserID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserModel)obj);
        }

        public UserModel()
        {

        }

        public static UserModel FromEmail(string email)
        {
            return new UserModel()
            {
                Email = email
            };
        }

        public static UserModel FromMailAddress(MailAddress adr)
        {
            return new UserModel()
            {
                Email = adr.Address,
                FirstName = adr.DisplayName
            };
        }

        public override int GetHashCode()
        {
            return UserID.GetHashCode();
        }

        public Guid UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        public string Company { get; set; }

        public string Picture { get; set; }

        public string FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }

        public string SortableFullName
        {
            get { return (LastName + ", " + FirstName).TrimStart(',', ' '); }
        }

        public bool HideProjectsOnPublicProfile { get; set; }

        public bool DoNotSendGroupNotifications { get; set; }

        public string Greeting
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    return Email;
                }
                else
                {
                    return FirstName;
                }
            }
        }
    }
}