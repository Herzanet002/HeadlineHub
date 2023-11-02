namespace HeadlineHub.Domain.Entities;

public class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public DateTimeOffset RegistrationDate { get; set; }
}