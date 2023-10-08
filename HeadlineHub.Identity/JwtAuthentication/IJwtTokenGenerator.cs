using System.Security.Claims;

namespace HeadlineHub.Identity.JwtAuthentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(IEnumerable<Claim> claims);
}