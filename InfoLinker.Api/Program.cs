using Carter;
using InfoLinker.Api;
using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Implementations;
using InfoLinker.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISyndicationWorker, SyndicationWorker>();
builder.Services.AddSingleton<IRssWorkerService, RssWorkerService>();
builder.Services.Decorate<IRssWorkerService, RssWorkerCacheDecorator>();
builder.Services.Configure<List<RssFeeder>>(builder.Configuration.GetSection("FeedResources"));
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapCarter();
app.Run();