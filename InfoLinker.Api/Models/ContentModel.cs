namespace InfoLinker.Api.Models;

public record ContentModel
{ 
    public string? Title { get; set; }

    public string? Link { get; set; }

    public string? Description { get; set; }

    public string? PubDate { get; set; }

    public string? ImageUrl { get; set; }
    
    public Guid FeederId { get; set; }
    
    public ICollection<string> Categories { get; set; } = null!;
}