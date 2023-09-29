using System.ServiceModel.Syndication;

namespace HeadlineHub.Api.Models;

public class CustomSyndicationItem
{
    public Guid FeederId { get; init; }

    public SyndicationItem OriginalItem { get; init; } = null!;
}