using System.ServiceModel.Syndication;
using System.Xml;
using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace InfoLinker.Api.Services.Implementations;

public class RssWorkerService : IRssWorkerService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<RssFeeder> _rssFeeders;

    public RssWorkerService(IOptions<List<RssFeeder>> rssFeeders,
        IMemoryCache memoryCache,
        IHttpClientFactory httpClientFactory)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
        _rssFeeders = rssFeeders.Value;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeeds(int? pageIndex, int? pageSize)
    {
        // const string cacheKey = nameof(GetFeeds);
        // if (_memoryCache.TryGetValue<IEnumerable<ContentModel>>(cacheKey, out var cachedContentModels))
        // {
        //     return cachedContentModels!;
        // }
    
        var syndicationFeedsTasks = _rssFeeders.Select(feed => GetSyndicationFeedAsync(feed, pageIndex.Value, pageSize.Value));
        var syndicationFeeds = await Task.WhenAll(syndicationFeedsTasks).ConfigureAwait(false);

        var contentModels = new List<ContentModel>();
        foreach (var feed in syndicationFeeds)
        {
            contentModels.AddRange(feed.Select(CreateContentModel!));
        }

        // // Кэшируем результат на указанный период времени (например, 5 минут)
        // var cacheEntryOptions = new MemoryCacheEntryOptions
        // {
        //     AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        // };
        //
        // _memoryCache.Set(cacheKey, contentModels, cacheEntryOptions);

        return contentModels;
    }

    private static ContentModel CreateContentModel(CustomSyndicationItem element)
    {
        return new ContentModel
        {
            Title = element.OriginalItem.Title.Text,
            Description = element.OriginalItem.Summary?.Text.StripHtml().TrimStart().Replace("\n", string.Empty).TrimEnd(),
            PubDate = element.OriginalItem.PublishDate.LocalDateTime.ToString("g"),
            Link = element.OriginalItem.Links[0].Uri.ToString(),
            ImageUrl = element.OriginalItem.Links.Count == 2 ? element.OriginalItem.Links[1].Uri.ToString() : null,
            FeederId = element.FeederId,
            Categories = element.OriginalItem.Categories.Select(x => x.Name).ToList()
        };
    }

    private async Task<IEnumerable<CustomSyndicationItem?>> GetSyndicationFeedAsync(RssFeeder rssFeeder, int pageIndex, int pageSize)
    {
        var syndicationItems = new List<CustomSyndicationItem>();
        var mainItems = await ExtractSyndicationItems(rssFeeder, pageIndex, pageSize).ConfigureAwait(false);
        if (mainItems != null)
        {
            syndicationItems.AddRange(mainItems);
        }

        if (rssFeeder.CategorizedFeeds == null || !rssFeeder.CategorizedFeeds.Any())
        {
            return syndicationItems;
        }

        foreach (var categorizedFeed in rssFeeder.CategorizedFeeds)
        {
            var feed = await ExtractSyndicationItems(categorizedFeed, pageIndex, pageSize).ConfigureAwait(false);
            if (feed != null)
            {
                syndicationItems.AddRange(feed);
            }
        }

        return syndicationItems;
    }

    private async ValueTask<IEnumerable<CustomSyndicationItem>?> ExtractSyndicationItems(CategorizedFeed rssFeeder, int? pageIndex, int? pageSize)
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