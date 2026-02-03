using CFFFusions.Models;

namespace CFFFusions.Services;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
     Task SaveAsync(User user);
}
