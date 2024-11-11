using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController: ControllerBase
    {
        [HttpPost]
        public ActionResult<Guid> Register([FromBody] RegisterRequest request)
        {
            var newUserId = Guid.NewGuid();
            return Ok(newUserId);
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            return Ok();
        }

        [HttpGet]
        public ActionResult<List<UserResponse>> GetUsers()
        {
            List<UserResponse> users = new List<UserResponse>();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<UserWithCoursesResponse> GetUserById([FromRoute] Guid id)
        {
            var user = new UserWithCoursesResponse();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUserProfile([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser([FromRoute] Guid id)
        {
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public ActionResult DeactivateUser([FromRoute] Guid id)
        {
            return NoContent();
        }
    }
}
