
namespace OnlineLearningPlatform.BLL
{
    public interface IUsersManager
    {
        Task<string?> CheckCredentials(string login, string password);
    }
}