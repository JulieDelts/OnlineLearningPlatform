using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.ServicesUtils;

namespace OnlineLearningPlatform.BLL;

public class EnrollmentsService(
    IEnrollmentsRepository enrollmentsRepository,
    EnrollmentsUtils enrollmentsUtils,
    UsersUtils usersUtils,
    CoursesUtils coursesUtils
    ) : IEnrollmentsService
{
    public async Task EnrollAsync(Guid courseId, Guid userId)
    {
        var courseDTO = await coursesUtils.GetCourseByIdAsync(courseId);

        if (courseDTO.IsDeactivated)
            throw new EntityConflictException($"Course with id {courseId} is deactivated.");

        var userDTO = await usersUtils.GetUserByIdAsync(userId);

        if (userDTO.IsDeactivated)
            throw new EntityConflictException($"User with id {userId} is deactivated.");

        if (userDTO.Role != Role.Student)
            throw new EntityConflictException("The role of the user is not correct.");

        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(courseId, userId);

        if (enrollmentDTO != null)
            throw new EntityConflictException($"Enrollment with user id {userId} and course id {courseId} already exists.");

        var newEnrollment = new Enrollment()
        {
            Course = courseDTO,
            User = userDTO
        };

        await enrollmentsRepository.EnrollAsync(newEnrollment);
    }

    public async Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance, Guid teacherId)
    {
        var enrollmentDTO = await enrollmentsUtils.GetEnrollmentAsync(enrollment.CourseId, enrollment.UserId);

        if (teacherId != enrollmentDTO.Course.TeacherId)
            throw new AuthorizationFailedException("Users are only allowed to control attendance of students from their own course.");

        if (enrollmentDTO.User.IsDeactivated)
            throw new EntityConflictException($"User with id {enrollment.UserId} is deactivated.");

        if (enrollmentDTO.Course.IsDeactivated)
            throw new EntityConflictException($"Course with id {enrollment.CourseId} is deactivated.");

        var numberOfLesons = enrollmentDTO.Course.NumberOfLessons;

        if (attendance < 0 || attendance > numberOfLesons)
            throw new EntityConflictException("The attendance is out of the acceptable range.");

        await enrollmentsRepository.ControlAttendanceAsync(enrollmentDTO, attendance);
    }

    public async Task DisenrollAsync(EnrollmentManagementModel enrollment)
    {
        var enrollmentDTO = await enrollmentsUtils.GetEnrollmentAsync(enrollment.CourseId, enrollment.UserId);

        await enrollmentsRepository.DisenrollAsync(enrollmentDTO);
    }

    public async Task GradeStudentAsync(EnrollmentManagementModel enrollment, int grade, Guid teacherId)
    {
        var enrollmentDTO = await enrollmentsUtils.GetEnrollmentAsync(enrollment.CourseId, enrollment.UserId);

        if (teacherId != enrollmentDTO.Course.TeacherId)
            throw new AuthorizationFailedException("Users are only allowed to grade students from their own course.");

        if (enrollmentDTO.User.IsDeactivated)
            throw new EntityConflictException($"User with id {enrollment.UserId} is deactivated.");

        if (enrollmentDTO.Course.IsDeactivated)
            throw new EntityConflictException($"Course with id {enrollment.CourseId} is deactivated.");

        await enrollmentsRepository.GradeStudentAsync(enrollmentDTO, grade);
    }

    public async Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review)
    {
        var enrollmentDTO = await enrollmentsUtils.GetEnrollmentAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO.User.IsDeactivated)
            throw new EntityConflictException($"User with id {enrollment.UserId} is deactivated.");

        if (enrollmentDTO.Course.IsDeactivated)
            throw new EntityConflictException($"Course with id {enrollment.CourseId} is deactivated.");

        await enrollmentsRepository.ReviewCourseAsync(enrollmentDTO, review);
    }
}
