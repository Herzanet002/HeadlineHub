using HeadlineHub.Application.Interfaces.Services;
using HeadlineHub.Domain.Common;
using HeadlineHub.Infrastructure.Common.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HeadlineHub.Infrastructure.Services.RssWorker;

public class RssWorkerCacheDecorator : IRssWorkerService
{
    private readonly IRssWorkerService _rssWorkerService;
    private readonly IMemoryCache _memoryCache;
    private readonly CacheSettings _cacheSettings;

    public RssWorkerCacheDecorator(IRssWorkerService rssWorkerService,
        IMemoryCache memoryCache,
        IOptionsMonitor<CacheSettings> cacheSettings)
    {
        _rssWorkerService = rssWorkerService;
        _memoryCache = memoryCache;
        _cacheSettings = cacheSettings.CurrentValue;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeedsAsync(PageInfo pageInfo)
    {
        var cacheKey = $"{nameof(GetFeedsAsync)}_{pageInfo.Index}_{pageInfo.Size}";

        if (_memoryCache.TryGetValue<IEnumerable<ContentModel>>(cacheKey, out var cachedContentModels))
        {
            return cachedContentModels!;
        }

        var contentModels = (await _rssWorkerService.GetFeedsAsync(pageInfo)).ToList();
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheSettings.CacheTtl
        };

        _memoryCache.Set(cacheKey, contentModels, cacheEntryOptions);

        return contentModels;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeedsAsync(IEnumerable<Guid> feedersIds, PageInfo pageInfo)
    {
        var cacheKey = $"{nameof(GetFeedsAsync)}_{string.Join("_", feedersIds)}_{pageInfo.Index}_{pageInfo.Size}";

        if (_memoryCache.TryGetValue<IEnumerable<ContentModel>>(cacheKey, out var cachedContentModels))
        {
            return cachedContentModels!;
        }

        var contentModels = (await _rssWorkerService.GetFeedsAsync(feedersIds, pageInfo)).ToList();
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheSettings.CacheTtl
        };

        _memoryCache.Set(cacheKey, contentModels, cacheEntryOptions);

        return contentModels;
    }

    public IEnumerable<RssFeeder> GetRssFeeders()
        => _rssWorkerService.GetRssFeeders();
}