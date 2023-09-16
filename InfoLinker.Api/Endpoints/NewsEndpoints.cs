using Carter;
using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InfoLinker.Api.Endpoints;

public class NewsEndpoints : CarterModule
{
    private readonly IRssWorkerService _rssWorkerService;
    private readonly IOptions<List<RssFeeder>> _rssFeeders;
    private const string ApiRoutePath = "/api";

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{ApiRoutePath}/news", GetPaginatedNews);
        app.MapGet($"{ApiRoutePath}/news-providers", GetRssFeeders);
        app.MapGet($"{ApiRoutePath}/news/{{feederId:guid}}", GetPaginatedNewsByFeeder);
    }

    public NewsEndpoints(IRssWorkerService rssWorkerService, IOptions<List<RssFeeder>> rssFeeders)
    {
        _rssWorkerService = rssWorkerService;
        _rssFeeders = rssFeeders;
    }

    private async ValueTask<IEnumerable<ContentModel>> GetPaginatedNews(int? pageSize, int? pageIndex)
        => await _rssWorkerService.GetFeeds(pageIndex, pageSize);

    private IEnumerable<RssFeeder> GetRssFeeders() => _rssFeeders.Value;

    private async ValueTask<IEnumerable<ContentModel>> GetPaginatedNewsByFeeder(Guid feederId, int? pageSize, int? pageIndex)
        => await _rssWorkerService.GetFeeds(feederId, pageSize, pageIndex);
}