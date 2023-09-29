namespace HeadlineHub.Api.Models;

public record ContentModel
{
    public Guid FeederId { get; set; }

    public string? Title { get; set; }

    public string? Link { get; set; }

    public string? Description { get; set; }

    public DateTime? PubDate { get; set; }

    public string? ImageUrl { get; set; }

    public ICollection<string>? Categories { get; set; }
}