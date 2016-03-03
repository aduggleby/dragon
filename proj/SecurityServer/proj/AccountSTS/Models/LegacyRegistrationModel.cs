using System;
using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.AccountSTS.Models
{
    [Table("LegacyLocalAccount")]
    public class LegacyRegistrationModel
    {
        [Key]
        public Guid UserId { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
    }
}