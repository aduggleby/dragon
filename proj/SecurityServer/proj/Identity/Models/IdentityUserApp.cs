using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.Identity.Models
{
    [Table("IdentityConsumerUser")]
    public class IdentityUserApp
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string AppId { get; set; }
    }
}
