using System.Text;
using HeadlineHub.Application.Common.Extensions;
using HeadlineHub.Application.Interfaces.Repositories;
using HeadlineHub.Application.Interfaces.Services;
using HeadlineHub.Infrastructure.Common.Configurations;
using HeadlineHub.Infrastructure.JwtAuthentication;
using HeadlineHub.Infrastructure.JwtAuthentication.Options;
using HeadlineHub.Infrastructure.Repositories;
using HeadlineHub.Infrastructure.Services;
using HeadlineHub.Infrastructure.Services.RssWorker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HeadlineHub.Infrastructure;

public static class DependencyInjection
{
    public static void AddHeadlineHubInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        #region Identity

        var jwtOptions = new JwtOptions();
        configuration.Bind(nameof(JwtOptions), jwtOptions);

        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.AddSingleton<ITokenGenerator, TokenGenerator>();

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

        #endregion

        #region Workers

        services.AddMemoryCache();
        services.Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)));
        services.AddSingleton<ISyndicationWorker, SyndicationWorker>();
        services.AddSingleton<IRssWorkerService, RssWorkerService>();
        services.Decorate<IRssWorkerService, RssWorkerCacheDecorator>();

        #endregion

        #region Repositories

        services.AddSingleton<IUsersRepository, UsersRepository>();

        #endregion

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddSingleton(new SqlConnectorFactory(connectionString!));
    }
}