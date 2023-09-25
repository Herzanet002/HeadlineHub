using InfoLinker.Api.Models;
using InfoLinker.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace InfoLinker.Api.Services.Implementations;

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

    public async ValueTask<IEnumerable<ContentModel>> GetFeeds(int? pageIndex, int? pageSize)
    {
        var cacheKey = $"{nameof(GetFeeds)}_{pageIndex}_{pageSize}";

        if (_memoryCache.TryGetValue<IEnumerable<ContentModel>>(cacheKey, out var cachedContentModels))
        {
            return cachedContentModels!;
        }

        var contentModels = (await _rssWorkerService.GetFeeds(pageIndex, pageSize)).ToList();
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheSettings.CacheTtl
        };

        _memoryCache.Set(cacheKey, contentModels, cacheEntryOptions);

        return contentModels;
    }

    public async ValueTask<IEnumerable<ContentModel>> GetFeeds(IEnumerable<Guid> feedersIds, int? pageIndex,
        int? pageSize)
    {
        var cacheKey = $"{nameof(GetFeeds)}_{string.Join("_", feedersIds)}_{pageIndex}_{pageSize}";

        if (_memoryCache.TryGetValue<IEnumerable<ContentModel>>(cacheKey, out var cachedContentModels))
        {
            return cachedContentModels!;
        }

        var contentModels = (await _rssWorkerService.GetFeeds(feedersIds, pageIndex, pageSize)).ToList();
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheSettings.CacheTtl
        };

        _memoryCache.Set(cacheKey, contentModels, cacheEntryOptions);

        return contentModels;
    }
}