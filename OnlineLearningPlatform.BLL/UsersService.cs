using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;

        private readonly Mapper _mapper;

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

        public async Task<Guid?> Register(UserRegistrationModel userToRegister)
        {
            var user = await _repository.GetUserByLogin(userToRegister.Login);

            if (user == null)
            {
                var userDTO = _mapper.Map<User>(userToRegister);

                userDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userToRegister.Password);

                var newId = await _repository.Register(userDTO);

                return newId;
            }
            else
            {
                return null;
            }
        }

        public async Task<string?> Authenticate(string login, string password)
        {
            var user = await _repository.GetUserByLogin(login);

            if (user != null && CheckPassword(password, user.Password))
            {
                return GenerateToken(user);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            var userDTOs = await _repository.GetAllUsers();

            var users = _mapper.Map<List<UserModel>>(userDTOs);

            return users;

        }

        public async Task<ExtendedUserModel> GetUserById(Guid id)
        {
            var userDTO = await _repository.GetUserByIdWithFullInfo(id);

            var user = _mapper.Map<ExtendedUserModel>(userDTO);

            List<CourseModel> courses = _mapper.Map<List<CourseModel>>(userDTO.TaughtCourses);

            user.TaughtCourses = courses;

            List<CourseEnrollmentModel> enrollments = _mapper.Map<List<CourseEnrollmentModel>>(userDTO.Enrollments);

            user.Enrollments = enrollments;

            return user;
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
}

