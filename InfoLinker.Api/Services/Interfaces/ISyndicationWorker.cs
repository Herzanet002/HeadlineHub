using InfoLinker.Api.Models;

namespace InfoLinker.Api.Services.Interfaces;

public interface ISyndicationWorker
{
    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, PageInfo pageInfo);

    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(CategorizedFeeder rssFeeder, PageInfo pageInfo);
}