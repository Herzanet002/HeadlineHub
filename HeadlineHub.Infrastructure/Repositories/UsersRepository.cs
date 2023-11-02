using Dapper;
using HeadlineHub.Application.Interfaces.Repositories;
using HeadlineHub.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HeadlineHub.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly SqlConnectorFactory _connectorFactory;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(SqlConnectorFactory connectorFactory, ILogger<UsersRepository> logger)
    {
        _connectorFactory = connectorFactory;
        _logger = logger;
    }

    public async Task RegisterAsync(string login)
    {
        using var connection = _connectorFactory.CreateConnection();
        try
        {
            await connection.ExecuteAsync(
                "INSERT INTO Users (user_name) VALUES (@username)",
                new { username = login });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user {username}", login);
            throw;
        }
    }

    public async Task<User?> GetByLoginAsync(string username)
    {
        using var connection = _connectorFactory.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT TOP 1 user_name UserName, registration_date RegistrationDate, user_id UserId " +
            "FROM dbo.users WHERE user_name = @username",
            new { username });
        return user;
    }
}