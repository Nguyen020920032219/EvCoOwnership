using System;
using System.Collections.Generic;

namespace AuthService.Data.Entities;

public partial class AuthToken
{
    public int Id { get; set; }

    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public bool? Valid { get; set; }

    public DateTimeOffset ExpiredAt { get; set; }

    public int? UserId { get; set; }

    public DateTimeOffset RefreshTokenExpiredAt { get; set; }

    public virtual AppUser? User { get; set; }
}
