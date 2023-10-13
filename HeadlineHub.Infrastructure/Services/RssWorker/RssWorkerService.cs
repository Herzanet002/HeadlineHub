using HeadlineHub.Application.Common.Extensions;
using HeadlineHub.Application.Interfaces.Services;
using HeadlineHub.Domain.Common;
using Microsoft.Extensions.Options;

namespace HeadlineHub.Infrastructure.Services.RssWorker;

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

    public async ValueTask<IEnumerable<ContentModel>> GetFeedsAsync(PageInfo pageInfo)
    {
        var syndicationFeedsTasks =
            _rssFeeders.Select(feed => _syndicationWorker.GetSyndicationFeedAsync(feed, pageInfo));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks.Select(x => x)).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(item => item!.CreateContentModel()));
        }

        return contentModels;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeedsAsync(IEnumerable<Guid> feedersIds, PageInfo pageInfo)
    {
        var suitableFeeders = FindSuitableFeeders(_rssFeeders, feedersIds);
        var syndicationFeedsTasks = suitableFeeders
            .Select(feed => _syndicationWorker.GetSyndicationFeedAsync(feed, pageInfo));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(item => item!.CreateContentModel()));
        }

        return contentModels;
    }

    public IEnumerable<RssFeeder> GetRssFeeders()
        => _rssFeeders;

    private static IEnumerable<CategorizedFeeder> FindSuitableFeeders(IEnumerable<RssFeeder> feeders,
        IEnumerable<Guid> feederIds)
    {
        var intersectedMain = feeders.IntersectBy(feederIds, feeder => feeder.Id);
        var categorizedFeeds = feeders.Where(rssFeeder => rssFeeder.CategorizedFeeders != null)
            .SelectMany(rssFeeder => rssFeeder.CategorizedFeeders!);
        var intersectedSecond = categorizedFeeds.IntersectBy(feederIds, feeder => feeder.Id);
        return intersectedMain.Union(intersectedSecond);
    }
}