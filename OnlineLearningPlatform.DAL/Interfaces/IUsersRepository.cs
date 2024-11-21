using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> CheckCredentials(string login, string password);

        Task DeactivateUser(Guid id);

        Task DeleteUser(Guid id);

        Task<List<User>> GetAllUsers();

        Task<User> GetUserByIdWithFullInfo(Guid id);

        Task<Guid> Register(User user);

        Task UpdatePassword(Guid id, User user);

        Task UpdateProfile(Guid id, User user);

        Task UpdateRole(Guid id, User user);
    }
}