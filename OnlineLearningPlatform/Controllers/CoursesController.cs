using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;
using OnlineLearningPlatform.BLL.Interfaces;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/courses")]
    [Authorize]
    public class CoursesController: ControllerBase
    {
        private readonly ICoursesService _service; 

        public CoursesController(ICoursesService service) 
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<Guid> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var newCourseId = Guid.NewGuid();
            return Ok(newCourseId);
        }

        [HttpPost("{id}/enroll")]
        public IActionResult Enroll([FromBody] EnrollmentManagementRequest request)
        {
            return NoContent();
        }

        [HttpGet]
        public ActionResult<List<CourseResponse>> GetCourses()
        {
            var courses = new List<CourseResponse>();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public ActionResult<ExtendedCourseResponse> GetCourseById([FromRoute] Guid id)
        {
            var course = new ExtendedCourseResponse();
            return Ok(course);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCourse([FromRoute] Guid id, [FromBody] UpdateCourseRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/grade")]
        public IActionResult GradeStudent([FromBody] GradeStudentRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/attendance")]
        public IActionResult ControlAttendance([FromBody] ControlAttendanceRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/review")]
        public IActionResult ReviewCourse([FromBody] CourseReviewRequest request)
        {
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult DeactivateCourse([FromRoute] Guid id)
        {
            return NoContent();
        }

        [HttpDelete("{id}/disenroll")]
        public IActionResult Disenroll([FromBody] EnrollmentManagementRequest request)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCourse([FromRoute] Guid id)
        {
            return NoContent();
        }
    }
}
