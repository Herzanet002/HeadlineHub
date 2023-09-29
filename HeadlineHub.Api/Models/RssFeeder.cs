namespace HeadlineHub.Api.Models;

public record RssFeeder : CategorizedFeeder
{
    public string? FeederImageLink { get; set; }

    public List<CategorizedFeeder>? CategorizedFeeders { get; set; }
}