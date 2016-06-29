using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.Data.Attributes;
using Microsoft.AspNet.Identity;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;


namespace Dragon.SecurityServer.PermissionSTS.Models
{
    public class AppMember : IUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppMember> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom AppMember claims here
            return userIdentity;
        }

        [Key]
        public string Id { get; set; }
        [NoColumn]
        public string UserName { get; set; }
        [NoColumn]
        public string PasswordHash { get; set; }
        [NoColumn]
        public string SecurityStamp { get; set; }
        [NoColumn]
        public string Email { get; set; }
        [NoColumn]
        public bool EmailConfirmed { get; set; }
        [NoColumn]
        public bool TwoFactorEnabled { get; set; }
        [NoColumn]
        public DateTime? LockoutEndDateUtc { get; set; }
        [NoColumn]
        public DateTime? Created { get; set; }
        [NoColumn]
        public DateTime? Modified { get; set; }
        [NoColumn]
        public int AccessFailedCount { get; set; }
        [NoColumn]
        public bool LockoutEnabled { get; set; }
    }
}