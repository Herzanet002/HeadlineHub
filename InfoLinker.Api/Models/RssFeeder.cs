namespace InfoLinker.Api.Models;

public record RssFeeder : CategorizedFeed
{
    public string? FeederImageLink { get; set; }
    
    public List<CategorizedFeed>? CategorizedFeeds { get; set; }
}

public record CategorizedFeed
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string RssUrl { get; set; } = null!;

    public string Link { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;
}