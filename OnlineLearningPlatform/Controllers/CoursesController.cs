﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Interfaces;
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
    public async Task<ActionResult<Guid>> CreateCourseAsync([FromBody] CreateCourseRequest request)
    {
        var courseModel = mapper.Map<CreateCourseModel>(request);

        var newCourseId = await coursesService.CreateCourseAsync(courseModel);

        return Ok(newCourseId);
    }

    [HttpPost("{id}/enroll")]
    public async Task<IActionResult> EnrollAsync([FromRoute] Guid id, [FromBody] EnrollmentManagementRequest request)
    {
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
    public async Task<ActionResult<List<UserEnrollmentResponse>>> GetEnrollmentsByCourseIdAsync([FromRoute] Guid id)
    {
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
    public async Task<IActionResult> UpdateCourseAsync([FromRoute] Guid id, [FromBody] UpdateCourseRequest request)
    {
        var courseModel = mapper.Map<UpdateCourseModel>(request);

        await coursesService.UpdateCourseAsync(id, courseModel);

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

        await enrollmentsService.GradeStudentAsync(enrollment, request.Grade);

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

        await enrollmentsService.ControlAttendanceAsync(enrollment, request.Attendance);

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

        await enrollmentsService.ReviewCourseAsync(enrollment, request.Review);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateCourseAsync([FromRoute] Guid id)
    {
        await coursesService.DeactivateCourseAsync(id);

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

        await enrollmentsService.DisenrollAsync(enrollment);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourseAsync([FromRoute] Guid id)
    {
        await coursesService.DeleteCourseAsync(id);

        return NoContent();
    }
}
