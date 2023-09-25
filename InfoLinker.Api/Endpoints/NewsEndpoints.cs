using Carter;
using FluentValidation;
using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InfoLinker.Api.Endpoints;

public class NewsEndpoints : CarterModule
{
    private readonly IRssWorkerService _rssWorkerService;
    private readonly IOptionsMonitor<List<RssFeeder>> _rssFeeders;
    private readonly IValidator<PageInfo> _pageInfoValidator;
    private const string ApiRoutePath = "/api";

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{ApiRoutePath}/news", GetPaginatedNews);
        app.MapGet($"{ApiRoutePath}/news-providers", GetRssFeeders);
        app.MapPost($"{ApiRoutePath}/news", GetPaginatedNewsByFeeders);
    }

    public NewsEndpoints(IRssWorkerService rssWorkerService, IOptionsMonitor<List<RssFeeder>> rssFeeders,
        IValidator<PageInfo> pageInfoValidator)
    {
        _rssWorkerService = rssWorkerService;
        _rssFeeders = rssFeeders;
        _pageInfoValidator = pageInfoValidator;
    }

    private async ValueTask<IEnumerable<ContentModel>> GetPaginatedNews(int? pageSize, int? pageIndex)
    {
        var pageInfo = new PageInfo(pageIndex, pageSize);
        await _pageInfoValidator.ValidateAndThrowAsync(pageInfo);
        return await _rssWorkerService.GetFeeds(pageInfo);
    }

    private IEnumerable<RssFeeder> GetRssFeeders()
        => _rssFeeders.CurrentValue;

    private async ValueTask<IEnumerable<ContentModel>> GetPaginatedNewsByFeeders(
        [FromBody] GetNewsByFeedersDto feedersDto, int? pageSize, int? pageIndex)
    {
        var pageInfo = new PageInfo(pageIndex, pageSize);
        await _pageInfoValidator.ValidateAndThrowAsync(pageInfo);
        return await _rssWorkerService.GetFeeds(feedersDto.Ids, pageInfo);
    }
}