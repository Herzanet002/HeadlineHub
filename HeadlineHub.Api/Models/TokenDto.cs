namespace HeadlineHub.Api.Models;

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    DateTime ExpiresAt,
    TimeSpan TokenLifetime
);