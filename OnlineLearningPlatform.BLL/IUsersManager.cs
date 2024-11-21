using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL
{
    public interface IUsersManager
    {
        Task<User?> CheckCredentials(string login, string password);
    }
}