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

        private readonly IMapper _mapper;

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
            var registrationModel = _mapper.Map<UserRegistrationModel>(request);

            var newId = await _service.Register(registrationModel);

            return Ok(newId);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var token = await _service.Authenticate(request.Login, request.Password);

            return Ok(token);
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
        public async Task<IActionResult> UpdateUserProfile([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            var profile = _mapper.Map<UpdateUserProfileModel>(request);

            await _service.UpdateProfile(id, profile);

            return NoContent();
        }

        [HttpPatch("{id}/role")]
        public async Task<IActionResult> UpdateUserRole([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request)
        {
            await _service.UpdateRole(id, request.Role);

            return NoContent();
        }

        [HttpPatch("{id}/password")]
        public IActionResult UpdateUserPassword([FromRoute] Guid id, [FromBody] UpdateUserPasswordRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser([FromRoute] Guid id)
        {
            await _service.DeactivateUser(id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            await _service.DeleteUser(id);

            return NoContent();
        }
    }
}
