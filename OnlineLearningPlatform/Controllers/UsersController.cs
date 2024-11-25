using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Mappings;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers;

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
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterRequest request)
    {
        var registrationModel = _mapper.Map<UserRegistrationModel>(request);

        var newId = await _service.RegisterAsync(registrationModel);

        return Ok(newId);
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await _service.AuthenticateAsync(request.Login, request.Password);

        return Ok(token);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsersAsync()
    {
        var users = await _service.GetAllUsersAsync();

        var response = _mapper.Map<List<UserResponse>>(users);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExtendedUserResponse>> GetUserByIdAsync([FromRoute] Guid id)
    {
        var user = await _service.GetUserByIdAsync(id);

        var response = _mapper.Map<ExtendedUserResponse>(user);

        return Ok(response);
    }

    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateUserProfileAsync([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        var profile = _mapper.Map<UpdateUserProfileModel>(request);

        await _service.UpdateProfileAsync(id, profile);

        return NoContent();
    }

    [HttpPatch("{id}/role")]
    public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        await _service.UpdateRoleAsync(id, request.Role);

        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdateUserPasswordAsync([FromRoute] Guid id, [FromBody] UpdateUserPasswordRequest request)
    {
        var passwordModel = _mapper.Map<UpdateUserPasswordModel>(request);

        await _service.UpdatePasswordAsync(id, passwordModel);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUserAsync([FromRoute] Guid id)
    {
        await _service.DeactivateUserAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
        await _service.DeleteUserAsync(id);

        return NoContent();
    }
}
