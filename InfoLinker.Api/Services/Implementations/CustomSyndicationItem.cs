using System.ServiceModel.Syndication;

namespace InfoLinker.Api.Services.Implementations;

public class CustomSyndicationItem
{
    public Guid FeederId { get; init; }
    
    public SyndicationItem OriginalItem { get; init; } = null!;
}