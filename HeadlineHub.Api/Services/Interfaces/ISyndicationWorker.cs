using HeadlineHub.Api.Models;

namespace HeadlineHub.Api.Services.Interfaces;

public interface ISyndicationWorker
{
    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, PageInfo pageInfo);

    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(CategorizedFeeder rssFeeder, PageInfo pageInfo);
}