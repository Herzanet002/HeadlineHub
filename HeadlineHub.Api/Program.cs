using Carter;
using FluentValidation;
using HeadlineHub.Api;
using HeadlineHub.Api.Models;
using HeadlineHub.Api.Services.Implementations;
using HeadlineHub.Api.Services.Interfaces;
using HeadlineHub.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("providers.json");

ConfigureSwaggerGen(builder);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ISyndicationWorker, SyndicationWorker>();
builder.Services.AddSingleton<IRssWorkerService, RssWorkerService>();
builder.Services.Decorate<IRssWorkerService, RssWorkerCacheDecorator>();
builder.Services.Configure<List<RssFeeder>>(builder.Configuration.GetSection("FeedResources"));
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(nameof(CacheSettings)));
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(PageInfo), ServiceLifetime.Singleton);
builder.Services.AddHeadlineHubIdentity(builder.Configuration);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
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