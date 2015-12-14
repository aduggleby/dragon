using System;

namespace Dragon.SecurityServer.GenericSTSClient.Test.Models
{
    public class UserModel
    {
        public long? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AppId { get; set; }
        public Guid ServiceId { get; set; }
        public Boolean Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
