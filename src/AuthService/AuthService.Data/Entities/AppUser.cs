using System;
using System.Collections.Generic;

namespace AuthService.Data.Entities;

public partial class AppUser
{
    public int UserId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<AuthToken> AuthTokens { get; set; } = new List<AuthToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual UserProfile? UserProfile { get; set; }
}
