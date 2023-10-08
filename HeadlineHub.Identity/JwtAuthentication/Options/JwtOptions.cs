namespace HeadlineHub.Identity.JwtAuthentication.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public string Secret { get; set; } = null!; 
    
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(30);
}