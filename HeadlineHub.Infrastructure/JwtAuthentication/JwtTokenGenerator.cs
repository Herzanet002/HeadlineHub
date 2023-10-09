using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HeadlineHub.Infrastructure.JwtAuthentication.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HeadlineHub.Infrastructure.JwtAuthentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerator(IOptionsMonitor<JwtOptions> jwtOptions) 
        => _jwtOptions = jwtOptions.CurrentValue;

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var jwt = new JwtSecurityToken
        (
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtOptions.TokenLifetime),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)),
                SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}