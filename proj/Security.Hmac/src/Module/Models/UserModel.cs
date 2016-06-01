using System;
using System.Globalization;

namespace Dragon.Security.Hmac.Module.Models
{
    public class UserModel
    {
        public long? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AppId { get; set; }
        public Guid ServiceId { get; set; }
        public Boolean Enabled { get; set; }
        public DateTime CreatedAt { get; set; }

        protected bool Equals(UserModel other)
        {
            return Id == other.Id && UserId.Equals(other.UserId) && AppId.Equals(other.AppId) && ServiceId.Equals(other.ServiceId) && Enabled.Equals(other.Enabled) && CreatedAt.ToString(CultureInfo.InvariantCulture).Equals(other.CreatedAt.ToString(CultureInfo.InvariantCulture));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ UserId.GetHashCode();
                hashCode = (hashCode*397) ^ AppId.GetHashCode();
                hashCode = (hashCode*397) ^ ServiceId.GetHashCode();
                hashCode = (hashCode*397) ^ Enabled.GetHashCode();
                hashCode = (hashCode*397) ^ CreatedAt.ToString(CultureInfo.InvariantCulture).GetHashCode();
                return hashCode;
            }
        }
    }
}
