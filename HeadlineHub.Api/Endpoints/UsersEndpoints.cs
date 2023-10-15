using System.Security.Claims;
using Carter;
using HeadlineHub.Application.Interfaces.Repositories;
using HeadlineHub.Infrastructure.JwtAuthentication;
using Microsoft.AspNetCore.Mvc;

namespace HeadlineHub.Api.Endpoints;

public class UsersEndpoints : CarterModule
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUsersRepository _usersRepository;

    public UsersEndpoints(IJwtTokenGenerator jwtTokenGenerator, 
        IUsersRepository usersRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _usersRepository = usersRepository;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{ApiConstants.ApiRoutePath}/login", LoginUser);
        app.MapPost($"{ApiConstants.ApiRoutePath}/register", RegisterUser);
    }

    private async Task<IResult> LoginUser([FromQuery] string username)
    {
        var user = await _usersRepository.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            return Results.NotFound();
        }
        var claims = new Claim(ClaimTypes.Name, username);
        var token = _jwtTokenGenerator.GenerateToken(new[] { claims });
        return Results.Ok(token);

    }
    
    private async Task RegisterUser([FromBody] string username)
    {
        await _usersRepository.TryRegisterAsync(username);
    }
}