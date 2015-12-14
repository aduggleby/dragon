using System;
using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.Identity.Models
{
    public class IdentityUserClaim
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        [Length(1000)]
        public string ClaimType { get; set; }
        [Length(1000)]
        public string ClaimValue { get; set; }

        public IdentityUserClaim()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
