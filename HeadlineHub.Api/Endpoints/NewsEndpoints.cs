using Carter;
using FluentValidation;
using HeadlineHub.Api.Models;
using HeadlineHub.Application.Interfaces.Services;
using HeadlineHub.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HeadlineHub.Api.Endpoints;

public class NewsEndpoints : CarterModule
{
    private readonly IRssWorkerService _rssWorkerService;
    private readonly IOptionsMonitor<List<RssFeeder>> _rssFeeders;
    private readonly IValidator<PageInfo> _pageInfoValidator;

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{ApiConstants.ApiRoutePath}/news", GetPaginatedNews);
        app.MapGet($"{ApiConstants.ApiRoutePath}/news-providers", GetRssFeeders);
        app.MapPost($"{ApiConstants.ApiRoutePath}/news", GetPaginatedNewsByFeeders);
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