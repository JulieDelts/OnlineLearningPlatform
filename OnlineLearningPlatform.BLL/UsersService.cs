using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class UsersService(
    IUsersRepository usersRepository,
    IMapper mapper
    ) : IUsersService
{
    public async Task<Guid> RegisterAsync(UserRegistrationModel userToRegister)
    {
        var user = await usersRepository.GetUserByLoginAsync(userToRegister.Login);

        if (user == null)
        {
            var userDTO = mapper.Map<User>(userToRegister);

            userDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userToRegister.Password);

            var newId = await usersRepository.RegisterAsync(userDTO);

            return newId;
        }
        else
        {
            throw new EntityConflictException($"User with login {userToRegister.Login} already exists.");
        }
    }

    public async Task<string> AuthenticateAsync(string login, string password)
    {
        var user = await usersRepository.GetUserByLoginAsync(login);

        if (user != null && CheckPassword(password, user.Password))
        {
            return GenerateToken(user);
        }
        else
        {
            throw new WrongCredentialsException("The credentials are not correct.");
        }
    }

    public async Task<List<UserModel>> GetAllActiveUsersAsync()
    {
        var userDTOs = await usersRepository.GetAllActiveUsersAsync();

        var users = mapper.Map<List<UserModel>>(userDTOs);

        return users;
    }

    public async Task<List<CourseModel>> GetTaughtCoursesByUserIdAsync(Guid id)
    {
        var user = await usersRepository.GetUserByIdWithFullInfoAsync(id);

        if (user == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        if (user.Role != Role.Teacher)
            throw new EntityConflictException("The role of the user is not correct.");

        var courses = mapper.Map<List<CourseModel>>(user.TaughtCourses);

        return courses;
    }

    public async Task<List<CourseEnrollmentModel>> GetEnrollmentsByUserIdAsync(Guid id)
    {
        var userDTO = await usersRepository.GetUserByIdWithFullInfoAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        if (userDTO.Role != Role.Student)
            throw new EntityConflictException("The role of the user is not correct.");

        var enrollments = mapper.Map<List<CourseEnrollmentModel>>(userDTO.Enrollments);

        return enrollments;
    }

    public async Task<ExtendedUserModel> GetUserByIdAsync(Guid id)
    {
        var userDTO = await usersRepository.GetUserByIdWithFullInfoAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found");

        var user = mapper.Map<ExtendedUserModel>(userDTO);

        return user;
    }

    public async Task UpdatePasswordAsync(Guid id, UpdateUserPasswordModel passwordModel)
    {
        var userDTO = await usersRepository.GetUserByIdAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        if (userDTO.IsDeactivated)
            throw new EntityConflictException($"User with id {id} is deactivated.");

        if (CheckPassword(passwordModel.CurrentPassword, userDTO.Password))
        {
            var password = BCrypt.Net.BCrypt.EnhancedHashPassword(passwordModel.NewPassword);

            await usersRepository.UpdatePasswordAsync(userDTO, password);
        }
        else
        {
            throw new WrongCredentialsException("The credentials are not correct.");
        }
    }

    public async Task UpdateProfileAsync(Guid id, UpdateUserProfileModel profileModel)
    {
        var userProfileDTO = mapper.Map<User>(profileModel);

        var userDTO = await usersRepository.GetUserByIdAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        if (userDTO.IsDeactivated)
            throw new EntityConflictException($"User with id {id} is deactivated.");

        await usersRepository.UpdateProfileAsync(userDTO, userProfileDTO);
    }

    public async Task UpdateRoleAsync(Guid id, Role role)
    {
        var userDTO = await usersRepository.GetUserByIdWithFullInfoAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        if (userDTO.IsDeactivated)
            throw new EntityConflictException($"User with id {id} is deactivated.");

        if (userDTO.Role != Role.Student || userDTO.Enrollments.Count > 0)
            throw new EntityConflictException($"User with id {id} does not satisfy the requirements.");

        await usersRepository.UpdateRoleAsync(userDTO, role);
    }

    public async Task DeactivateUserAsync(Guid id)
    {
        var userDTO = await usersRepository.GetUserByIdAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        await usersRepository.DeactivateUserAsync(userDTO);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var userDTO = await usersRepository.GetUserByIdAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        await usersRepository.DeleteUserAsync(userDTO);
    }

    private bool CheckPassword(string passwordToCheck, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, passwordHash);
    }

    private string GenerateToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthConfigOptions.Key));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: AuthConfigOptions.Issuer,
            audience: AuthConfigOptions.Audience,
            claims: new List<Claim>()
            {
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("SystemId", user.Id.ToString())
            },
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}

