using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;

        public UsersService(IUsersRepository repository)
        {
            _repository = repository;
        }

        public async Task<string?> CheckCredentials(string login, string password)
        {
            var user = await _repository.CheckCredentials(login, password);

            if (user != null)
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
            else
            {
                return null;
            }
        }
    }
}
