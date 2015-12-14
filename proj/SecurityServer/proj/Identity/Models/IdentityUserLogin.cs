using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.Identity.Models
{
    public class IdentityUserLogin
    {
        [Key]
        [Length(128)]
        public string LoginProvider { get; set; }
        [Key]
        [Length(128)]
        public string ProviderKey { get; set; }
        [Key]
        [Length(128)]
        public string UserId { get; set; }
    }
}
