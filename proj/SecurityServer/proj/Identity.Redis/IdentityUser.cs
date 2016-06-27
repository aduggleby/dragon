using System;
using StackRedis.AspNet.Identity;

namespace Dragon.SecurityServer.Identity.Redis
{
    public class IdentityUser : IIdentityUser, Dragon.SecurityServer.Identity.Models.IUser, Microsoft.AspNet.Identity.IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
    }
}
