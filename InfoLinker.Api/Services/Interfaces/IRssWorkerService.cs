using InfoLinker.Api.Models;

namespace InfoLinker.Api.Services.Interfaces;

public interface IRssWorkerService
{
    ValueTask<IEnumerable<ContentModel>> GetFeeds(PageInfo pageInfo);

    ValueTask<IEnumerable<ContentModel>> GetFeeds(IEnumerable<Guid> feedersIds, PageInfo pageInfo);
}