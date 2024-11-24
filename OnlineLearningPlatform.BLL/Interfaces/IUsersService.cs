using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.BLL.Interfaces
{
    public interface IUsersService
    {
        Task<Guid> Register(UserRegistrationModel user);

        Task<string> Authenticate(string login, string password);

        Task<ExtendedUserModel> GetUserById(Guid id);

        Task<List<UserModel>> GetAllUsers();

        Task UpdateRole(Guid id, Role role);

        Task UpdateProfile(Guid id, UpdateUserProfileModel profileModel);

        Task UpdatePassword(Guid id, UpdateUserPasswordModel passwordModel);

        Task DeactivateUser(Guid id);

        Task DeleteUser(Guid id);
    }
}