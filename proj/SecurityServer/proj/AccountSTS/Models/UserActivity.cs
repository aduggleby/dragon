using System;
using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.AccountSTS.Models
{
    [Table("UserActivity")]
    public class UserActivity
    {
        public string UserId { get; set; }
        public string AppId { get; set; }
        public string ServiceId { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
    }
}
