using InternInventory.Models;
using InternInventory.Repositories;
using BCrypt.Net;

namespace InternInventory.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.Status != "Active")
            {
                return null;
            }

            // Verify using BCrypt
            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordMatch)
            {
                return null;
            }

            return user;
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            // Validate username uniqueness
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return false;
            }

            // Hash the password using BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedDate = DateTime.UtcNow;
            user.Status = "Active";

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }
    }
}
