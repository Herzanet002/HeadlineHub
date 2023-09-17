using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InfoLinker.Api.Services.Implementations;

public class RssWorkerService : IRssWorkerService
{
    private readonly ISyndicationWorker _syndicationWorker;
    private readonly IEnumerable<RssFeeder> _rssFeeders;

    public RssWorkerService(IOptions<List<RssFeeder>> rssFeeders,
        ISyndicationWorker syndicationWorker)
    {
        _syndicationWorker = syndicationWorker;
        _rssFeeders = rssFeeders.Value;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeeds(int? pageIndex, int? pageSize)
    {
        var syndicationFeedsTasks =
            _rssFeeders.Select(feed => _syndicationWorker.GetSyndicationFeedAsync(feed, pageIndex, pageSize));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks.Select(x => x)).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(item => item!.CreateContentModel()));
        }
        return contentModels;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeeds(IEnumerable<Guid> feedersIds, int? pageIndex,
        int? pageSize)
    {
        var suitableFeeders = FindSuitableFeeders(_rssFeeders, feedersIds);
        var syndicationFeedsTasks = suitableFeeders
            .Select(feed => _syndicationWorker.GetSyndicationFeedAsync(feed, pageIndex, pageSize));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(item => item!.CreateContentModel()));
        }

        return contentModels;
    }
    
    private static IEnumerable<CategorizedFeed> FindSuitableFeeders(IEnumerable<RssFeeder> feeders, IEnumerable<Guid> feederIds)
    {
        var intersectedMain = feeders.IntersectBy(feederIds, feeder => feeder.Id);
        var categorizedFeeds = feeders.Where(rssFeeder => rssFeeder.CategorizedFeeds != null)
            .SelectMany(rssFeeder => rssFeeder.CategorizedFeeds!);
        var intersectedSecond = categorizedFeeds.IntersectBy(feederIds, feeder => feeder.Id);
        return intersectedMain.Union(intersectedSecond);
    }
}