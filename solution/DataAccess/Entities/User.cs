using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Role { get; set; } = null!;

    public int PersonId { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
