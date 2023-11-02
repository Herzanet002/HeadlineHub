namespace HeadlineHub.Domain.Common;

public record CategorizedFeeder
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string RssUrl { get; set; } = null!;

    public string Link { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;
}