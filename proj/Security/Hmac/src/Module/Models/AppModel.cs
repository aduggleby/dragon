using System;

namespace Dragon.Security.Hmac.Module.Models
{
    public class AppModel
    {
        public long? Id { get; set; }
        public Guid AppId { get; set; }
        public Guid ServiceId { get; set; }
        public string Secret { get; set; }
        public Boolean Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
    }
}
