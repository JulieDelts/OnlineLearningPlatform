using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;

    private readonly IMapper _mapper;

    public UsersService(IUsersRepository repository)
    {
        _repository = repository;

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new BLLUserMapperProfile());
                cfg.AddProfile(new BLLCourseMapperProfile());
            });
        _mapper = new Mapper(config);
    }

    public async Task<Guid> RegisterAsync(UserRegistrationModel userToRegister)
    {
        var user = await _repository.GetUserByLoginAsync(userToRegister.Login);

        if (user == null)
        {
            var userDTO = _mapper.Map<User>(userToRegister);

            userDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userToRegister.Password);

            var newId = await _repository.RegisterAsync(userDTO);

            return newId;
        }
        else
        {
            throw new EntityConflictException($"User with login {userToRegister.Login} already exists.");
        }
    }

    public async Task<string> AuthenticateAsync(string login, string password)
    {
        var user = await _repository.GetUserByLoginAsync(login);

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
        var userDTOs = await _repository.GetAllUsersAsync();

        var users = _mapper.Map<List<UserModel>>(userDTOs);

        return users;
    }

    public async Task<ExtendedUserModel> GetUserByIdAsync(Guid id)
    {
        var userDTO = await _repository.GetUserByIdWithFullInfoAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found");
        }

        var user = _mapper.Map<ExtendedUserModel>(userDTO);

        List<CourseModel> courses = _mapper.Map<List<CourseModel>>(userDTO.TaughtCourses);

        user.TaughtCourses = courses;

        List<CourseEnrollmentModel> enrollments = _mapper.Map<List<CourseEnrollmentModel>>(userDTO.Enrollments);

        user.Enrollments = enrollments;

        return user;
    }

    public async Task UpdatePasswordAsync(Guid id, UpdateUserPasswordModel passwordModel)
    {
        var userDTO = await _repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        if (CheckPassword(passwordModel.CurrentPassword, userDTO.Password))
        {
            var password = BCrypt.Net.BCrypt.EnhancedHashPassword(passwordModel.NewPassword);

            await _repository.UpdatePasswordAsync(userDTO, password);
        }
        else
        {
            throw new WrongCredentialsException("The credentials are not correct.");
        }
    }

    public async Task UpdateProfileAsync(Guid id, UpdateUserProfileModel profileModel)
    {
        var userProfileDTO = _mapper.Map<User>(profileModel);

        var userDTO = await _repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await _repository.UpdateProfileAsync(userDTO, userProfileDTO);
    }

    public async Task UpdateRoleAsync(Guid id, Role role)
    {
        var userDTO = await _repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await _repository.UpdateRoleAsync(userDTO, role);
    }

    public async Task DeactivateUserAsync(Guid id)
    {
        var userDTO = await _repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await _repository.DeactivateUserAsync(userDTO);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var userDTO = await _repository.GetUserByIdAsync(id);

        if (userDTO == null)
        {
            throw new EntityNotFoundException($"User with id {id} was not found.");
        }

        await _repository.DeleteUserAsync(userDTO);
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

