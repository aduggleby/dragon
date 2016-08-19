using Dragon.Data.Attributes;

namespace Dragon.SecurityServer.AccountSTS.Models
{
    [Table("ConsumerInfo")]
    public class AppInfo
    {
        [Key]
        public string AppId { get; set; }
        public string GroupId { get; set; }
        public string Url { get; set; }
    }
}
