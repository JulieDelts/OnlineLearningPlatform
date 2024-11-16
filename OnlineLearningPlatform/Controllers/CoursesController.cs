using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController: ControllerBase
    {
        [HttpPost]
        public ActionResult<Guid> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var newCourseId = Guid.NewGuid();
            return Ok(newCourseId);
        }

        [HttpGet]
        public ActionResult<List<ExtendedCourseResponse>> GetCourses()
        {
            var courses = new List<ExtendedCourseResponse>();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public ActionResult<CourseWithUsersResponse> GetCourseById([FromRoute] Guid id)
        {
            var course = new UserWithCoursesResponse();
            return Ok(course);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCourse([FromRoute] Guid id, [FromBody] UpdateCourseRequest request)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCourse([FromRoute] Guid id)
        {
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public ActionResult DeactivateCourse([FromRoute] Guid id)
        {
            return NoContent();
        }
    }
}
