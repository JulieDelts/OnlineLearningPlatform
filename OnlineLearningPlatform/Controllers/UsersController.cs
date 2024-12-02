using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUsersService service, IMapper mapper) : ControllerBase
{
    [HttpPost, AllowAnonymous]
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterRequest request)
    {
        var registrationModel = mapper.Map<UserRegistrationModel>(request);

        var newId = await service.RegisterAsync(registrationModel);

        return Ok(newId);
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await service.AuthenticateAsync(request.Login, request.Password);

        return Ok(token);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsersAsync()
    {
        var users = await service.GetAllUsersAsync();

        var response = mapper.Map<List<UserResponse>>(users);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExtendedUserResponse>> GetUserByIdAsync([FromRoute] Guid id)
    {
        var user = await service.GetUserByIdAsync(id);

        var response = mapper.Map<ExtendedUserResponse>(user);

        return Ok(response);
    }

    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateUserProfileAsync([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        var profile = mapper.Map<UpdateUserProfileModel>(request);

        await service.UpdateProfileAsync(id, profile);

        return NoContent();
    }

    [HttpPatch("{id}/role")]
    public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        await service.UpdateRoleAsync(id, request.Role);

        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdateUserPasswordAsync([FromRoute] Guid id, [FromBody] UpdateUserPasswordRequest request)
    {
        var passwordModel = mapper.Map<UpdateUserPasswordModel>(request);

        await service.UpdatePasswordAsync(id, passwordModel);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUserAsync([FromRoute] Guid id)
    {
        await service.DeactivateUserAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
        await service.DeleteUserAsync(id);

        return NoContent();
    }
}
