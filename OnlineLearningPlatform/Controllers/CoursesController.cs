using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;
using OnlineLearningPlatform.BLL.Interfaces;
using AutoMapper;
using OnlineLearningPlatform.Mappings;
using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.Controllers;

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
    public async Task<ActionResult<Guid>> CreateCourseAsync([FromBody] CreateCourseRequest request)
    {
        var courseModel = _mapper.Map<CreateCourseModel>(request);

        var newCourseId = await _service.CreateCourseAsync(courseModel);

        return Ok(newCourseId);
    }

    [HttpPost("{id}/enroll")]
    public async Task<IActionResult> EnrollAsync([FromRoute] Guid id, [FromBody] EnrollmentManagementRequest request)
    {
        await _service.EnrollAsync(id, request.UserId);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseResponse>>> GetCoursesAsync()
    {
        var courseModels = await _service.GetAllCoursesAsync();

        var courses = _mapper.Map<List<CourseResponse>>(courseModels);

        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExtendedCourseResponse>> GetCourseByIdAsync([FromRoute] Guid id)
    {
        var courseModel = await _service.GetCourseByIdAsync(id);

        var course = _mapper.Map<ExtendedCourseResponse>(courseModel);

        return Ok(course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourseAsync([FromRoute] Guid id, [FromBody] UpdateCourseRequest request)
    {
        var courseModel = _mapper.Map<UpdateCourseModel>(request);

        await _service.UpdateCourseAsync(id, courseModel);

        return NoContent();
    }

    [HttpPatch("{id}/grade")]
    public async Task<IActionResult> GradeStudentAsync([FromRoute] Guid id, [FromBody] GradeStudentRequest request)
    {
        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        await _service.GradeStudentAsync(enrollment, request.Grade);

        return NoContent();
    }

    [HttpPatch("{id}/attendance")]
    public async Task<IActionResult> ControlAttendanceAsync([FromRoute] Guid id, [FromBody] ControlAttendanceRequest request)
    {
        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        await _service.ControlAttendanceAsync(enrollment, request.Attendance);

        return NoContent();
    }

    [HttpPatch("{id}/review")]
    public async Task<IActionResult> ReviewCourseAsync([FromRoute] Guid id, [FromBody] CourseReviewRequest request)
    {
        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        await _service.ReviewCourseAsync(enrollment, request.Review);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateCourseAsync([FromRoute] Guid id)
    {
        await _service.DeactivateCourseAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}/disenroll")]
    public async Task<IActionResult> DisenrollAsync([FromRoute] Guid id, [FromBody] EnrollmentManagementRequest request)
    {
        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        await _service.DisenrollAsync(enrollment);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourseAsync([FromRoute] Guid id)
    {
        await _service.DeleteCourseAsync(id);

        return NoContent();
    }
}
