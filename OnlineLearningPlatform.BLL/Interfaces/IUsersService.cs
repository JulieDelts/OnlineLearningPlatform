using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces
{
    public interface IUsersService
    {
        Task<Guid?> Register(UserRegistrationModel user);

        Task<string?> Authenticate(string login, string password);

        Task<ExtendedUserModel> GetUserById(Guid id);

        Task<List<UserModel>> GetAllUsers();
    }
}