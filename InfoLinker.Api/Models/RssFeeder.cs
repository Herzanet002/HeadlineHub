namespace InfoLinker.Api.Models;

public record RssFeeder : CategorizedFeed
{
    public string? FeederImageLink { get; set; }

    public List<CategorizedFeed>? CategorizedFeeds { get; set; }
}