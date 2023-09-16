using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Implementations;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRssWorkerService, RssWorkerService>();
builder.Services.Configure<List<RssFeeder>>(builder.Configuration.GetSection("FeedResources"));
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("news", 
    async (IRssWorkerService rssWorkerService, int? pageSize, int? pageIndex) 
        => await rssWorkerService.GetFeeds(pageIndex, pageSize));

app.MapGet("news-providers",(IOptions<List<RssFeeder>> rssFeeders) => rssFeeders.Value);

app.Run();