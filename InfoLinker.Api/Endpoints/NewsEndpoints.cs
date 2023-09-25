using Carter;
using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InfoLinker.Api.Endpoints;

public class NewsEndpoints : CarterModule
{
    private readonly IRssWorkerService _rssWorkerService;
    private readonly IOptionsMonitor<List<RssFeeder>> _rssFeeders;
    private const string ApiRoutePath = "/api";

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{ApiRoutePath}/news", GetPaginatedNews);
        app.MapGet($"{ApiRoutePath}/news-providers", GetRssFeeders);
        app.MapPost($"{ApiRoutePath}/news", GetPaginatedNewsByFeeders);
    }

    public NewsEndpoints(IRssWorkerService rssWorkerService, IOptionsMonitor<List<RssFeeder>> rssFeeders)
    {
        _rssWorkerService = rssWorkerService;
        _rssFeeders = rssFeeders;
    }

    private async ValueTask<IEnumerable<ContentModel>> GetPaginatedNews(int? pageSize, int? pageIndex)
        => await _rssWorkerService.GetFeeds(pageIndex, pageSize);

    private IEnumerable<RssFeeder> GetRssFeeders()
        => _rssFeeders.CurrentValue;

    private async ValueTask<IEnumerable<ContentModel>> GetPaginatedNewsByFeeders(
        [FromBody] GetNewsByFeedersDto feedersDto, int? pageSize, int? pageIndex)
        => await _rssWorkerService.GetFeeds(feedersDto.Ids, pageIndex, pageSize);
}