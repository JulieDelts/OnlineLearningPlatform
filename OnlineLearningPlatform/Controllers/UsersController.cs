﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private IUsersService _service;

        public UsersController() 
        { 
            _service = new UsersService();
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

            var token = await _service.CheckCredentials(request.Login, request.Password);

            if (token != null)
            {
                return Ok(new AuthenticatedResponse { Token = token });
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
