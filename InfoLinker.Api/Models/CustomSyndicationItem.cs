using System.ServiceModel.Syndication;

namespace InfoLinker.Api.Models;

public class CustomSyndicationItem
{
    public Guid FeederId { get; init; }

    public SyndicationItem OriginalItem { get; init; } = null!;
}