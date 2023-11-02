using FluentValidation;
using HeadlineHub.Api.Models;
using HeadlineHub.Application.Interfaces.Services;
using HeadlineHub.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HeadlineHub.Api.Endpoints;

public static class NewsEndpoints
{
    public static void MapEndpoints(WebApplication app, string endpointBaseAddress, string[] openApiTags)
    {
        app.MapGet(endpointBaseAddress, async (
            [FromServices] IValidator<PageInfo> pageInfoValidator,
            [FromServices] IRssWorkerService rssWorkerService,
            int? pageSize, int? pageIndex) =>
        {
            var pageInfo = new PageInfo(pageIndex, pageSize);
            await pageInfoValidator.ValidateAndThrowAsync(pageInfo);
            return await rssWorkerService.GetFeedsAsync(pageInfo);
        }).WithTags(openApiTags).RequireAuthorization();

        app.MapGet("news-providers", (
                [FromServices] IRssWorkerService rssWorkerService)
            => rssWorkerService.GetRssFeeders()).WithTags(openApiTags);

        app.MapPost(endpointBaseAddress, async (
            [FromServices] IValidator<PageInfo> pageInfoValidator,
            [FromServices] IRssWorkerService rssWorkerService,
            [FromBody] GetNewsByFeedersDto feedersDto,
            int? pageSize, int? pageIndex) =>
        {
            var pageInfo = new PageInfo(pageIndex, pageSize);
            await pageInfoValidator.ValidateAndThrowAsync(pageInfo);
            return await rssWorkerService.GetFeedsAsync(feedersDto.Ids, pageInfo);
        }).WithTags(openApiTags).RequireAuthorization();
    }
}