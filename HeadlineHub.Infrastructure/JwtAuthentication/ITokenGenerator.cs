using System.Security.Claims;

namespace HeadlineHub.Infrastructure.JwtAuthentication;

public interface ITokenGenerator
{
    string GenerateJwtAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredJwtToken(string token); 
    
    TimeSpan GetTokenLifetime();
}