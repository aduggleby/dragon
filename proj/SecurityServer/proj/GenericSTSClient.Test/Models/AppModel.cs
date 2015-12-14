using System;

namespace Dragon.SecurityServer.GenericSTSClient.Test.Models
{
    public class AppModel
    {
        public int? Id { get; set; }
        public Guid AppId { get; set; }
        public Guid ServiceId { get; set; }
        public string Secret { get; set; }
        public Boolean Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
    }
}
