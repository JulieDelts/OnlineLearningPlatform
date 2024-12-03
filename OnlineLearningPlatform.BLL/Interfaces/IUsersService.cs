using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.BLL.Interfaces;

public interface IUsersService
{
    Task<Guid> RegisterAsync(UserRegistrationModel user);

    Task<string> AuthenticateAsync(string login, string password);

    Task<ExtendedUserModel> GetUserByIdAsync(Guid id);

    Task<List<UserModel>> GetAllUsersAsync();

    Task UpdateRoleAsync(Guid id, Role role);

    Task UpdateProfileAsync(Guid id, UpdateUserProfileModel profileModel);

    Task UpdatePasswordAsync(Guid id, UpdateUserPasswordModel passwordModel);

    Task DeactivateUserAsync(Guid id);

    Task DeleteUserAsync(Guid id);

    Task<List<UserModel>> GetStudentsByCourseIdAsync(Guid id);
}