
namespace OnlineLearningPlatform.BLL
{
    public interface IUsersService
    {
        Task<string?> CheckCredentials(string login, string password);
    }
}