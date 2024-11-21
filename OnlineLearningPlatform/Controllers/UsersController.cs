using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController: ControllerBase
    {
        private IUsersManager _manager;

        public UsersController() 
        { 
            _manager = new UsersManager();
        }

        [HttpPost, AllowAnonymous]
        public ActionResult<Guid> Register([FromBody] RegisterRequest request)
        {
            var newUserId = Guid.NewGuid();
            return Ok(newUserId);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<AuthenticatedResponse>> Login([FromBody] LoginRequest request)
        {
            if (request is null)
            {
                return BadRequest("The login request is invalid.");
            }

            var user = await _manager.CheckCredentials(request.Login, request.Password);

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
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new AuthenticatedResponse { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public ActionResult<List<UserResponse>> GetUsers()
        {
            List<UserResponse> users = new List<UserResponse>();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<ExtendedUserResponse> GetUserById([FromRoute] Guid id)
        {
            var user = new ExtendedUserResponse();
            return Ok(user);
        }

        [HttpPut("{id}/profile")]
        public IActionResult UpdateUserProfile([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/role")]
        public IActionResult UpdateUserRole([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/password")]
        public IActionResult UpdateUserPassword([FromRoute] Guid id, [FromBody] UpdateUserPasswordRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult DeactivateUser([FromRoute] Guid id)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] Guid id)
        {
            return NoContent();
        }
    }
}
