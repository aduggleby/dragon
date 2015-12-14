using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.Identity.Models
{
    public class IdentityUserRole
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string RoleId { get; set; }
    }
}
