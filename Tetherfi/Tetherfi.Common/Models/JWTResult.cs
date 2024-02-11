using System;

namespace Tetherfi.Common.Models;

public class JWTResult
{
    public string Token { get; set; }

    public DateTime Expiration { get; set; }

    public User User { get; set; }

    public string RefreshToken { get; set; }
}