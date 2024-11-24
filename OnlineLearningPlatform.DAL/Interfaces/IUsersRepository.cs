using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> GetUserByLogin(string login);

        Task DeactivateUser(Guid id);

        Task DeleteUser(Guid id);

        Task<List<User>> GetAllUsers();

        Task<User> GetUserByIdWithFullInfo(Guid id);

        Task<User> GetUserById(Guid id);

        Task<Guid> Register(User user);

        Task UpdatePassword(Guid id, string password);

        Task UpdateProfile(Guid id, User user);

        Task UpdateRole(Guid id, Role role);
    }
}