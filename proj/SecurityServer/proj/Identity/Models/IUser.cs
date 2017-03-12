using System;

namespace Dragon.SecurityServer.Identity.Models
{
    public interface IUser : Microsoft.AspNet.Identity.IUser<string>
    {
        new string Id { get; set; }
        string PasswordHash { get; set; }
        string SecurityStamp { get; set; }
        string Email { get; set; }
        bool EmailConfirmed { get; set; }
        bool TwoFactorEnabled { get; set; }
        DateTime? LockoutEndDateUtc { get; set; }
        DateTime? Created { get; set; }
        DateTime? Modified { get; set; }
        int AccessFailedCount { get; set; }
        bool LockoutEnabled { get; set; }
    }
}
