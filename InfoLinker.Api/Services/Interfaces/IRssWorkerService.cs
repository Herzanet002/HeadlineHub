using InfoLinker.Api.Models;

namespace InfoLinker.Api.Services.Interfaces;

public interface IRssWorkerService
{
    ValueTask<IEnumerable<ContentModel>> GetFeeds(int? pageIndex, int? pageSize);
}