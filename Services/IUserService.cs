using InternInventory.Models;

namespace InternInventory.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
