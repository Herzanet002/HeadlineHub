using HeadlineHub.Domain.Entities;

namespace HeadlineHub.Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<bool> TryRegisterAsync(string username);
    
    Task<User?> GetUserByUsernameAsync(string username);
}