using HeadlineHub.Domain.Common;

namespace HeadlineHub.Application.Interfaces.Services;

public interface IRssWorkerService
{
    ValueTask<IEnumerable<ContentModel>> GetFeeds(PageInfo pageInfo);

    ValueTask<IEnumerable<ContentModel>> GetFeeds(IEnumerable<Guid> feedersIds, PageInfo pageInfo);
}