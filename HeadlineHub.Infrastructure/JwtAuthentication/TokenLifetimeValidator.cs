using Microsoft.IdentityModel.Tokens;

namespace HeadlineHub.Infrastructure.JwtAuthentication;

public static class TokenLifetimeValidator
{
    public static bool Validate(
        DateTime? notBefore,
        DateTime? expires,
        SecurityToken tokenToValidate,
        TokenValidationParameters tokenValidationParameters)
        => expires != null && expires > DateTime.UtcNow;
}