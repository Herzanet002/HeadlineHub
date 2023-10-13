using HeadlineHub.Domain.Common;

namespace HeadlineHub.Application.Interfaces.Services;

public interface IRssWorkerService
{
    ValueTask<IEnumerable<ContentModel>> GetFeedsAsync(PageInfo pageInfo);

    ValueTask<IEnumerable<ContentModel>> GetFeedsAsync(IEnumerable<Guid> feedersIds, PageInfo pageInfo);

    IEnumerable<RssFeeder> GetRssFeeders();
}