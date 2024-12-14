using AutoMapper;
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
[Route("api/courses")]
[Authorize]
public class CoursesController(
    ICoursesService coursesService,
    IEnrollmentsService enrollmentsService,
    IMapper mapper
    ) : ControllerBase
{
    [HttpPost]
    [CustomAuthorize([Role.Teacher])]
    public async Task<ActionResult<Guid>> CreateCourseAsync([FromBody] CreateCourseRequest request)
    {
        var userId = this.GetUserIdFromClaims();

        if(request.TeacherId != userId)
            return Forbid();

        var courseModel = mapper.Map<CreateCourseModel>(request);

        var newCourseId = await coursesService.CreateCourseAsync(courseModel);

        return Ok(newCourseId);
    }

    [HttpPost("{id}/enroll")]
    [CustomAuthorize([Role.Student])]
    public async Task<IActionResult> EnrollAsync([FromRoute] Guid id, [FromBody] EnrollmentManagementRequest request)
    {
        var userId = this.GetUserIdFromClaims();

        if (request.UserId != userId)
            return Forbid();

        await enrollmentsService.EnrollAsync(id, request.UserId);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseResponse>>> GetActiveCoursesAsync()
    {
        var courseModels = await coursesService.GetAllActiveCoursesAsync();

        var courses = mapper.Map<List<CourseResponse>>(courseModels);

        return Ok(courses);
    }

    [HttpGet("{id}/enrollments")]
    [CustomAuthorize([Role.Teacher, Role.Admin])]
    public async Task<ActionResult<List<UserEnrollmentResponse>>> GetEnrollmentsByCourseIdAsync([FromRoute] Guid id)
    {
        var userId = this.GetUserIdFromClaims();

        var userRole = this.GetUserRoleFromClaims();

        if (userRole != Role.Admin && id != userId)
            return Forbid();

        var enrollmentModels = await coursesService.GetEnrollmentsByCourseIdAsync(id);

        var enrollments = mapper.Map<List<UserEnrollmentResponse>>(enrollmentModels);

        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExtendedCourseResponse>> GetCourseByIdAsync([FromRoute] Guid id)
    {
        var courseModel = await coursesService.GetFullCourseByIdAsync(id);

        var course = mapper.Map<ExtendedCourseResponse>(courseModel);

        return Ok(course);
    }

    [HttpPut("{id}")]
    [CustomAuthorize([Role.Teacher])]
    public async Task<IActionResult> UpdateCourseAsync([FromRoute] Guid id, [FromBody] UpdateCourseRequest request)
    {
        var courseModel = mapper.Map<UpdateCourseModel>(request);

        var teacherId = this.GetUserIdFromClaims();

        await coursesService.UpdateCourseAsync(id, courseModel, teacherId);

        return NoContent();
    }

    [HttpPatch("{id}/grade")]
    [CustomAuthorize([Role.Teacher])]
    public async Task<IActionResult> GradeStudentAsync([FromRoute] Guid id, [FromBody] GradeStudentRequest request)
    {
        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        var teacherId = this.GetUserIdFromClaims();

        await enrollmentsService.GradeStudentAsync(enrollment, request.Grade, teacherId);

        return NoContent();
    }

    [HttpPatch("{id}/attendance")]
    [CustomAuthorize([Role.Teacher])]
    public async Task<IActionResult> ControlAttendanceAsync([FromRoute] Guid id, [FromBody] ControlAttendanceRequest request)
    {
        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        var teacherId = this.GetUserIdFromClaims();

        await enrollmentsService.ControlAttendanceAsync(enrollment, request.Attendance, teacherId);

        return NoContent();
    }

    [HttpPatch("{id}/review")]
    [CustomAuthorize([Role.Student])]
    public async Task<IActionResult> ReviewCourseAsync([FromRoute] Guid id, [FromBody] CourseReviewRequest request)
    {
        var userId = this.GetUserIdFromClaims();

        if (request.UserId != userId)
            return Forbid();

        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        await enrollmentsService.ReviewCourseAsync(enrollment, request.Review);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    [CustomAuthorize([Role.Teacher])]
    public async Task<IActionResult> DeactivateCourseAsync([FromRoute] Guid id)
    {
        var teacherId = this.GetUserIdFromClaims();

        await coursesService.DeactivateCourseAsync(id, teacherId);

        return NoContent();
    }

    [HttpDelete("{id}/disenroll")]
    [CustomAuthorize([Role.Student])]
    public async Task<IActionResult> DisenrollAsync([FromRoute] Guid id, [FromBody] EnrollmentManagementRequest request)
    {
        var userId = this.GetUserIdFromClaims();

        if (request.UserId != userId)
            return Forbid();

        var enrollment = new EnrollmentManagementModel()
        {
            CourseId = id,
            UserId = request.UserId
        };

        await enrollmentsService.DisenrollAsync(enrollment);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> DeleteCourseAsync([FromRoute] Guid id)
    {
        await coursesService.DeleteCourseAsync(id);

        return NoContent();
    }
}
