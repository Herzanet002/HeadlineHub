namespace HeadlineHub.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<bool> TryLogin(string username, out IUser user);
    
    Task<bool> TryRegister(string username);
    
    Task<bool> TryLogout(IUser user);
}