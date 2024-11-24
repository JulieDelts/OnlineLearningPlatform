using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Mappings;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController: ControllerBase
    {
        private readonly IUsersService _service;

        private readonly Mapper _mapper;

        public UsersController(IUsersService service) 
        { 
            _service = service;
            var config = new MapperConfiguration(
               cfg =>
               {
                   cfg.AddProfile(new APIUserMapperProfile());
                   cfg.AddProfile(new APICourseMapperProfile());
               });
            _mapper = new Mapper(config);
        }

        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var registrationModel = _mapper.Map<UserRegistrationModel>(request);

            var newId = await _service.Register(registrationModel);

            if (newId == null)
            {
                return BadRequest();
            }

            return Ok(newId);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var token = await _service.Authenticate(request.Login, request.Password);

            if (token != null)
            {
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var users = await _service.GetAllUsers();

            var response = _mapper.Map<List<UserResponse>>(users);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExtendedUserResponse>> GetUserById([FromRoute] Guid id)
        {
            var user = await _service.GetUserById(id);

            var response = _mapper.Map<ExtendedUserResponse>(user);

            return Ok(response);
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
