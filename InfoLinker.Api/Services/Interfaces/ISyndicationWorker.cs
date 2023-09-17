using InfoLinker.Api.Models;

namespace InfoLinker.Api.Services.Interfaces;

public interface ISyndicationWorker
{
    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, int? pageIndex,
        int? pageSize);

    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(CategorizedFeed rssFeeder, int? pageIndex,
        int? pageSize);
}