using System;
using Dragon.Data.Attributes;
using Microsoft.AspNet.Identity;

namespace Dragon.SecurityServer.Identity.Models
{
    /// <summary>
    /// From <see href="http://www.asp.net/identity/overview/extensibility/overview-of-custom-storage-providers-for-aspnet-identity">asp.net</see>
    /// </summary>
    [Table("IdentityRole")]
    public class IdentityRole : IRole
    {
        public IdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string name): this()
        {
            Name = name;
        }

        public IdentityRole(string name, string id)
        {
            Name = name;
            Id = id;
        }

        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
