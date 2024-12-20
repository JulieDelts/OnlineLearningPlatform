﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Configuration;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(
    IUsersService usersService,
    IMapper mapper
    ) : ControllerBase
{
    [HttpPost, AllowAnonymous]
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterRequest request)
    {
        var registrationModel = mapper.Map<UserRegistrationModel>(request);

        var newId = await usersService.RegisterAsync(registrationModel);

        return Ok(newId);
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await usersService.AuthenticateAsync(request.Login, request.Password);

        return Ok(token);
    }

    [HttpGet]
    [CustomAuthorize([Role.Admin])]
    public async Task<ActionResult<List<UserResponse>>> GetAllActiveUsersAsync()
    {
        var users = await usersService.GetAllActiveUsersAsync();

        var response = mapper.Map<List<UserResponse>>(users);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExtendedUserResponse>> GetUserByIdAsync([FromRoute] Guid id)
    {
        var userId = this.GetUserIdFromClaims();

        var userRole = this.GetUserRoleFromClaims();

        if (userRole != Role.Admin && id != userId)
            return Forbid();

        var user = await usersService.GetUserByIdAsync(id);

        var response = mapper.Map<ExtendedUserResponse>(user);

        return Ok(response);
    }

    [HttpGet("{id}/taught-courses")]
    [CustomAuthorize([Role.Teacher, Role.Admin])]
    public async Task<ActionResult<List<CourseResponse>>> GetTaughtCoursesByUserIdAsync([FromRoute] Guid id)
    {
        var userId = this.GetUserIdFromClaims();

        var userRole = this.GetUserRoleFromClaims();

        if (userRole != Role.Admin && id != userId)
            return Forbid();

        var courses = await usersService.GetTaughtCoursesByUserIdAsync(id);

        var response = mapper.Map<List<CourseResponse>>(courses);

        return Ok(response);
    }

    [HttpGet("{id}/enrollments")]
    [CustomAuthorize([Role.Student, Role.Admin])]
    public async Task<ActionResult<List<CourseEnrollmentResponse>>> GetEnrollmentsByUserIdAsync([FromRoute] Guid id)
    {
        var userId = this.GetUserIdFromClaims();

        var userRole = this.GetUserRoleFromClaims();

        if (userRole != Role.Admin && id != userId)
            return Forbid();

        var enrollments = await usersService.GetEnrollmentsByUserIdAsync(id);

        var response = mapper.Map<List<CourseEnrollmentResponse>>(enrollments);

        return Ok(response);
    }

    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateUserProfileAsync([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        var userId = this.GetUserIdFromClaims();

        if (id != userId)
            return Forbid();

        var profile = mapper.Map<UpdateUserProfileModel>(request);

        await usersService.UpdateProfileAsync(id, profile);

        return NoContent();
    }

    [HttpPatch("{id}/role")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        await usersService.UpdateRoleAsync(id, request.Role);

        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdateUserPasswordAsync([FromRoute] Guid id, [FromBody] UpdateUserPasswordRequest request)
    {
        var userId = this.GetUserIdFromClaims();

        if (id != userId)
            return Forbid();

        var passwordModel = mapper.Map<UpdateUserPasswordModel>(request);

        await usersService.UpdatePasswordAsync(id, passwordModel);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUserAsync([FromRoute] Guid id)
    {
        var userId = this.GetUserIdFromClaims();

        if (id != userId)
            return Forbid();

        await usersService.DeactivateUserAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
        await usersService.DeleteUserAsync(id);

        return NoContent();
    }
}
