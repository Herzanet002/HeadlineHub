using System.Security.Claims;
using Carter;
using HeadlineHub.Infrastructure.JwtAuthentication;
using Microsoft.AspNetCore.Mvc;

namespace HeadlineHub.Api.Endpoints;

public class UsersEndpoints : CarterModule
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UsersEndpoints(IJwtTokenGenerator jwtTokenGenerator) 
        => _jwtTokenGenerator = jwtTokenGenerator;

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{ApiConstants.ApiRoutePath}/login", LoginUser);
    }

    private ActionResult<string> LoginUser([FromQuery] string username)
    {
        var claims = new Claim(ClaimTypes.Name, username);
        var token = _jwtTokenGenerator.GenerateToken(new[] { claims });
        return token;
    }
}