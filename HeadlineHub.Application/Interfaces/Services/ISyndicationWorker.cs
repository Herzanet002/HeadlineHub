using HeadlineHub.Domain.Common;

namespace HeadlineHub.Application.Interfaces.Services;

public interface ISyndicationWorker
{
    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, PageInfo pageInfo);

    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(CategorizedFeeder rssFeeder, PageInfo pageInfo);
}