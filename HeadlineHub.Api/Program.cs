using Carter;
using FluentValidation;
using HeadlineHub.Api;
using HeadlineHub.Api.Models;
using HeadlineHub.Api.Services.Implementations;
using HeadlineHub.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("providers.json");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISyndicationWorker, SyndicationWorker>();
builder.Services.AddSingleton<IRssWorkerService, RssWorkerService>();
builder.Services.Decorate<IRssWorkerService, RssWorkerCacheDecorator>();
builder.Services.Configure<List<RssFeeder>>(builder.Configuration.GetSection("FeedResources"));
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(nameof(CacheSettings)));
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(PageInfo), ServiceLifetime.Singleton);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapCarter();
app.Run();