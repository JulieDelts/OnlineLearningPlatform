namespace OnlineLearningPlatform.BLL.Interfaces
{
    public interface IUsersService
    {
        Task<string?> CheckCredentials(string login, string password);
    }
}