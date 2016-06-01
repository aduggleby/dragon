using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.Identity.Models
{
    public class IdentityService
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string ServiceId { get; set; }
    }
}
