using InfoLinker.Api.Models;

namespace InfoLinker.Api.Services.Implementations;

public interface ISyndicationWorker
{
    Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, int? pageIndex,
        int? pageSize);
}