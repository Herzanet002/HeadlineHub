using System.ServiceModel.Syndication;

namespace HeadlineHub.Domain.Common;

public record CustomSyndicationItem
{
    public Guid FeederId { get; init; }

    public SyndicationItem OriginalItem { get; init; } = null!;
}