﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ManagementWeb.Areas.Hmac.Models
{
    public class AppModel : IModel<int?>
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Provide a GUID in the format: 00000000-0000-0000-0000-000000000000")]
        public Guid AppId { get; set; }
        [Required(ErrorMessage = "Provide a GUID in the format: 00000000-0000-0000-0000-000000000000")]
        public Guid ServiceId { get; set; }
        public string Secret { get; set; }
        public Boolean Enabled { get; set; }
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Provide a date in the format: dd.mm.yyyy hh:mm:ss, e.g. 01.12.2001 23:34:42")]
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }

        protected bool Equals(AppModel other)
        {
            return Id == other.Id && AppId.Equals(other.AppId) && ServiceId.Equals(other.ServiceId) && string.Equals(Secret, other.Secret) && Enabled.Equals(other.Enabled) && CreatedAt.ToString(CultureInfo.InvariantCulture).Equals(other.CreatedAt.ToString(CultureInfo.InvariantCulture)) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AppModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ AppId.GetHashCode();
                hashCode = (hashCode*397) ^ ServiceId.GetHashCode();
                hashCode = (hashCode*397) ^ (Secret != null ? Secret.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Enabled.GetHashCode();
                hashCode = (hashCode*397) ^ CreatedAt.ToString(CultureInfo.InvariantCulture).GetHashCode();
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
