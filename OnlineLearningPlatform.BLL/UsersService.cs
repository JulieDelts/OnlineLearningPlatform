using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlineLearningPlatform.DAL;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL
{
    public class UsersService : IUsersService
    {
        private IUsersRepository _repository;

        public UsersService()
        {
            _repository = new UsersRepository();
        }

        public async Task<string?> CheckCredentials(string login, string password)
        {
            var user = await _repository.CheckCredentials(login, password);

            if (user != null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthConfigOptions.Key));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: AuthConfigOptions.Issuer,
                    audience: AuthConfigOptions.Audience,
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: signinCredentials
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
