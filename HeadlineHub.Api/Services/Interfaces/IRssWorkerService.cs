using HeadlineHub.Api.Models;

namespace HeadlineHub.Api.Services.Interfaces;

public interface IRssWorkerService
{
    ValueTask<IEnumerable<ContentModel>> GetFeeds(PageInfo pageInfo);

    ValueTask<IEnumerable<ContentModel>> GetFeeds(IEnumerable<Guid> feedersIds, PageInfo pageInfo);
}