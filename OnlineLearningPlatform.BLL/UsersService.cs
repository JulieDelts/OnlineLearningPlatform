using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.BLL.BusinessModels;
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

