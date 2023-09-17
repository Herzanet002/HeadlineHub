using System.ServiceModel.Syndication;
using System.Xml;
using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;

namespace InfoLinker.Api.Services.Implementations;

public class SyndicationWorker : ISyndicationWorker
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SyndicationWorker(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    public async Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, int? pageIndex,
        int? pageSize)
    {
        var syndicationItems = new List<CustomSyndicationItem>();
        var mainItems = await ExtractSyndicationItems(rssFeeder, pageIndex, pageSize)
            .ConfigureAwait(false);
        if (mainItems != null)
        {
            syndicationItems.AddRange(mainItems);
        }

        if (rssFeeder.CategorizedFeeders == null || !rssFeeder.CategorizedFeeders.Any())
        {
            return syndicationItems;
        }

        foreach (var categorizedFeed in rssFeeder.CategorizedFeeders)
        {
            var feed = await ExtractSyndicationItems(categorizedFeed, pageIndex, pageSize)
                .ConfigureAwait(false);
            if (feed != null)
            {
                syndicationItems.AddRange(feed);
            }
        }

        return syndicationItems;
    }

    public async Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(CategorizedFeeder rssFeeder,
        int? pageIndex, int? pageSize)
    {
        var syndicationItems = new List<CustomSyndicationItem>();
        var mainItems = await ExtractSyndicationItems(rssFeeder, pageIndex, pageSize)
            .ConfigureAwait(false);
        if (mainItems != null)
        {
            syndicationItems.AddRange(mainItems);
        }

        return syndicationItems;
    }

    private async ValueTask<IEnumerable<CustomSyndicationItem>?> ExtractSyndicationItems(CategorizedFeeder rssFeeder,
        int? pageIndex, int? pageSize)
    {
        try
        {
            if (!rssFeeder.IsEnabled)
            {
                return null;
            }

            using var client = _httpClientFactory.CreateClient();
            await using var response = await client.GetStreamAsync(rssFeeder.RssUrl).ConfigureAwait(false);
            using var reader = XmlReader.Create(response, new XmlReaderSettings { Async = true });

            var feed = SyndicationFeed.Load<SyndicationFeed>(reader);
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return feed.Items
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .Select(originalItem => new CustomSyndicationItem
                    {
                        FeederId = rssFeeder.Id,
                        OriginalItem = originalItem
                    });
            }

            return feed.Items
                .Select(originalItem => new CustomSyndicationItem
                {
                    FeederId = rssFeeder.Id,
                    OriginalItem = originalItem
                });
        }
        catch
        {
            return null;
        }
    }
}