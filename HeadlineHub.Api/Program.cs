using FluentValidation;
using HeadlineHub.Api;
using HeadlineHub.Api.Endpoints;
using HeadlineHub.Domain.Common;
using HeadlineHub.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("providers.json");

ConfigureSwaggerGen(builder);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<List<RssFeeder>>(builder.Configuration.GetSection("FeedResources"));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(IdentityConstants), ServiceLifetime.Singleton);
builder.Services.AddHttpClient();
builder.Services.AddHeadlineHubInfrastructure(builder.Configuration);

var app = builder.Build();
NewsEndpoints.MapEndpoints(app, "api/news", new[] { "News" });
ProfilesEndpoints.MapEndpoints(app, "api/profiles", new[] { "Profiles" });
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.Run();
return;

void ConfigureSwaggerGen(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddSwaggerGen(option =>
    {
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Bearer authorization",
            BearerFormat = "JWT",
            Scheme = "bearer",
            Description = "JWT Authorization header using the Bearer scheme.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http
        });

        var securityScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        });
    });
}