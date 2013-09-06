using System;
using System.Collections.Generic;
using System.Linq;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class Notification : INotification
    {
        public Guid ID { get; set; }
        public string TypeKey { get; set; }
        public string LanguageCode { get; set; }
        public Dictionary<string, string> Parameter { get; set; }
        public string Subject { get; set; }
        public bool Dispatched { get; set; }

        protected bool Equals(Notification other)
        {
            bool parameterEqual = Parameter.Count == other.Parameter.Count && !Parameter.Except(other.Parameter).Any();
            return parameterEqual && ID.Equals(other.ID) && string.Equals(TypeKey, other.TypeKey) && string.Equals(LanguageCode, other.LanguageCode) && string.Equals(Subject, other.Subject) && Dispatched.Equals(other.Dispatched);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Notification) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ID.GetHashCode();
                hashCode = (hashCode*397) ^ (TypeKey != null ? TypeKey.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LanguageCode != null ? LanguageCode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Dispatched.GetHashCode();
                return hashCode;
            }
        }
    }
}
