using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces;

public interface IUsersRepository
{
    Task<User?> GetUserByLoginAsync(string login);

    Task DeactivateUserAsync(User user);

    Task DeleteUserAsync(User user);

    Task<List<User>> GetAllUsersAsync();

    Task<User> GetUserByIdWithFullInfoAsync(Guid id);

    Task<User> GetUserByIdAsync(Guid id);

    Task<Guid> RegisterAsync(User user);

    Task UpdatePasswordAsync(User user, string password);

    Task UpdateProfileAsync(User user, User userUpdate);

    Task UpdateRoleAsync(User user, Role role);
}