using System.Text;
using HeadlineHub.Application.Common.Extensions;
using HeadlineHub.Application.Interfaces.Services;
using HeadlineHub.Infrastructure.JwtAuthentication;
using HeadlineHub.Infrastructure.JwtAuthentication.Options;
using HeadlineHub.Infrastructure.Services;
using HeadlineHub.Infrastructure.Services.RssWorker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HeadlineHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddHeadlineHubIdentity(this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        var jwtOptions = new JwtOptions();
        configuration.Bind(nameof(JwtOptions), jwtOptions);
        
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
                options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                RequireExpirationTime = true,
                LifetimeValidator = TokenLifetimeValidator.Validate,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Secret))
            });
        return services;
    }

    public static IServiceCollection AddHeadlineHubWorkers(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ISyndicationWorker, SyndicationWorker>();
        services.AddSingleton<IRssWorkerService, RssWorkerService>();
        services.Decorate<IRssWorkerService, RssWorkerCacheDecorator>();
        return services;
    }
}