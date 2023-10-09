using System.Security.Claims;

namespace HeadlineHub.Infrastructure.JwtAuthentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(IEnumerable<Claim> claims);
}