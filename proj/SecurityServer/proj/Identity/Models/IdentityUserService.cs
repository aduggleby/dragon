using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.Identity.Models
{
    [Table("IdentityServiceUser")]
    public class IdentityUserService
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string ServiceId { get; set; }
    }
}
