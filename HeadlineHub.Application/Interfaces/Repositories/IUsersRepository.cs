using HeadlineHub.Domain.Entities;

namespace HeadlineHub.Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task RegisterAsync(string login);
    
    Task<User?> GetByLoginAsync(string username);
}