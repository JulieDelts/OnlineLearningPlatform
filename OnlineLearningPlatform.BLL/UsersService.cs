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

public class UsersService(IUsersRepository repository, IMapper mapper) : IUsersService
{
    public async Task<Guid> RegisterAsync(UserRegistrationModel userToRegister)
    {
        var user = await repository.GetUserByLoginAsync(userToRegister.Login);

        if (user == null)
        {
            var userDTO = mapper.Map<User>(userToRegister);

            userDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userToRegister.Password);

            var newId = await repository.RegisterAsync(userDTO);

            return newId;
        }
        else
        {
            throw new EntityConflictException($"User with login {userToRegister.Login} already exists.");
        }
    }

    public async Task<string> AuthenticateAsync(string login, string password)
    {
        var user = await repository.GetUserByLoginAsync(login);

        if (user != null && CheckPassword(password, user.Password))
        {
            return GenerateToken(user);
        }
        else
        {
            throw new WrongCredentialsException("The credentials are not correct.");
        }
    }

    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        var userDTOs = await repository.GetAllUsersAsync();

        var users = mapper.Map<List<UserModel>>(userDTOs);

        return users;
    }

    public async Task<ExtendedUserModel> GetUserByIdAsync(Guid id)
    {
        var userDTO = await repository.GetUserByIdWithFullInfoAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found");
        }

        var user = mapper.Map<ExtendedUserModel>(userDTO);

        List<CourseModel> courses = mapper.Map<List<CourseModel>>(userDTO.TaughtCourses);

        user.TaughtCourses = courses;

        List<CourseEnrollmentModel> enrollments = mapper.Map<List<CourseEnrollmentModel>>(userDTO.Enrollments);

        user.Enrollments = enrollments;

        return user;
    }

    public async Task UpdatePasswordAsync(Guid id, UpdateUserPasswordModel passwordModel)
    {
        var userDTO = await repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        if (CheckPassword(passwordModel.CurrentPassword, userDTO.Password))
        {
            var password = BCrypt.Net.BCrypt.EnhancedHashPassword(passwordModel.NewPassword);

            await repository.UpdatePasswordAsync(userDTO, password);
        }
        else
        {
            throw new WrongCredentialsException("The credentials are not correct.");
        }
    }

    public async Task UpdateProfileAsync(Guid id, UpdateUserProfileModel profileModel)
    {
        var userProfileDTO = mapper.Map<User>(profileModel);

        var userDTO = await repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await repository.UpdateProfileAsync(userDTO, userProfileDTO);
    }

    public async Task UpdateRoleAsync(Guid id, Role role)
    {
        var userDTO = await repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await repository.UpdateRoleAsync(userDTO, role);
    }

    public async Task DeactivateUserAsync(Guid id)
    {
        var userDTO = await repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await repository.DeactivateUserAsync(userDTO);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var userDTO = await repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await repository.DeleteUserAsync(userDTO);
    }

    private bool CheckPassword(string passwordToCheck, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, passwordHash);
    }

    private string GenerateToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigOptions.Key));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: ConfigOptions.Issuer,
            audience: ConfigOptions.Audience,
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

