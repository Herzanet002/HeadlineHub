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
        var syndicationFeedsTasks = _rssFeeders.Select(feed => _syndicationWorker.GetSyndicationFeedAsync(feed, pageIndex, pageSize));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(item => item!.CreateContentModel()));
        }
        return contentModels;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeeds(IEnumerable<Guid> feedersIds, int? pageIndex, int? pageSize)
    {
        var syndicationFeedsTasks = _rssFeeders.IntersectBy(feedersIds, feeder => feeder.Id)
            .Select(feed => _syndicationWorker.GetSyndicationFeedAsync(feed, pageIndex, pageSize));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(item => item!.CreateContentModel()));
        }

        return contentModels;
    }
}