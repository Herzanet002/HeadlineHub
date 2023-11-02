using System.Security.Claims;
using HeadlineHub.Api.Models;
using HeadlineHub.Application.Interfaces.Repositories;
using HeadlineHub.Infrastructure.JwtAuthentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace HeadlineHub.Api.Endpoints;

public static class ProfilesEndpoints
{
    public static void MapEndpoints(WebApplication app, string endpointBaseAddress, string[] openApiTags)
    {
        app.MapGet(endpointBaseAddress + "/login", async (
            [FromServices] ITokenGenerator tokenGenerator,
            [FromServices] IUsersRepository usersRepository,
            [FromQuery] string login) =>
        {
            var user = await usersRepository.GetByLoginAsync(login);
            if (user == null)
            {
                return Results.NotFound();
            }

            var jwtAccessToken = tokenGenerator.GenerateJwtAccessToken(new[]
            {
                new Claim(IdentityConstants.Login, user.Username),
                new Claim(IdentityConstants.RegistrationDate, user.RegistrationDate.ToString())
            });
            
            var refreshToken = tokenGenerator.GenerateRefreshToken();
            var tokenLifetime = tokenGenerator.GetTokenLifetime();
            var token = new TokenDto(
                AccessToken: jwtAccessToken,
                RefreshToken: refreshToken,
                TokenType: JwtBearerDefaults.AuthenticationScheme,
                ExpiresAt: DateTime.UtcNow.Add(tokenLifetime),
                TokenLifetime: tokenLifetime);
            tokenGenerator.GetPrincipalFromExpiredJwtToken(jwtAccessToken);

            return Results.Ok(token);
        }).WithTags(openApiTags);

        app.MapPost(endpointBaseAddress + "/register", async (
            [FromServices] IUsersRepository usersRepository,
            [FromBody] RegisterUserDto user) =>
        {
            await usersRepository.RegisterAsync(user.Login);
        }).WithTags(openApiTags);
    }
}