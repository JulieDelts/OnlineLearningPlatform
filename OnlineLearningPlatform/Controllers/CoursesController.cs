using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;
using OnlineLearningPlatform.BLL.Interfaces;
using AutoMapper;
using OnlineLearningPlatform.Mappings;
using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/courses")]
    [Authorize]
    public class CoursesController: ControllerBase
    {
        private readonly ICoursesService _service;

        private readonly IMapper _mapper;

        public CoursesController(ICoursesService service) 
        {
            _service = service;

            var config = new MapperConfiguration(
              cfg =>
              {
                  cfg.AddProfile(new APICourseMapperProfile());
                  cfg.AddProfile(new APIUserMapperProfile());
              });
            _mapper = new Mapper(config);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var courseModel = _mapper.Map<CreateCourseModel>(request);

            var newCourseId = await _service.CreateCourse(courseModel);

            return Ok(newCourseId);
        }

        [HttpPost("{id}/enroll")]
        public IActionResult Enroll([FromBody] EnrollmentManagementRequest request)
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseResponse>>> GetCourses()
        {
            var courseModels = await _service.GetAllCourses();

            var courses = _mapper.Map<List<CourseResponse>>(courseModels);

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExtendedCourseResponse>> GetCourseById([FromRoute] Guid id)
        {
            var courseModel = await _service.GetCourseById(id);

            var course = _mapper.Map<ExtendedCourseResponse>(courseModel);

            return Ok(course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] UpdateCourseRequest request)
        {
            var courseModel = _mapper.Map<UpdateCourseModel>(request);

            await _service.UpdateCourse(id, courseModel);

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
        public async Task<IActionResult> DeactivateCourse([FromRoute] Guid id)
        {
            await _service.DeactivateCourse(id);

            return NoContent();
        }

        [HttpDelete("{id}/disenroll")]
        public IActionResult Disenroll([FromBody] EnrollmentManagementRequest request)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {
            await _service.DeleteCourse(id);

            return NoContent();
        }
    }
}
