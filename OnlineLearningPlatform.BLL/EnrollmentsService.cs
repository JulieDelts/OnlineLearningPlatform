using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.BLL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class EnrollmentsService(
    IUsersRepository usersRepository,
    ICoursesRepository coursesRepository,
    IEnrollmentsRepository enrollmentsRepository
    ) : IEnrollmentsService
{
    public async Task EnrollAsync(Guid courseId, Guid userId)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(courseId);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {courseId} was not found.");

        if (courseDTO.IsDeactivated)
            throw new EntityConflictException($"Course with id {courseId} is deactivated.");

        var userDTO = await usersRepository.GetUserByIdAsync(userId);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {userId} was not found.");

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

    public async Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        if (enrollmentDTO.User.IsDeactivated)
            throw new EntityConflictException($"User with id {enrollment.UserId} is deactivated.");

        if (enrollmentDTO.Course.IsDeactivated)
            throw new EntityConflictException($"Course with id {enrollment.CourseId} is deactivated.");

        var numberOfLesons = enrollmentDTO.Course.NumberOfLessons;

        if (attendance < 0 || attendance > numberOfLesons)
            throw new ArgumentException("The attendance is out of the acceptable range.");

        await enrollmentsRepository.ControlAttendanceAsync(enrollmentDTO, attendance);
    }

    public async Task DisenrollAsync(EnrollmentManagementModel enrollment)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        await enrollmentsRepository.DisenrollAsync(enrollmentDTO);
    }

    public async Task GradeStudentAsync(EnrollmentManagementModel enrollment, int grade)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        if (enrollmentDTO.User.IsDeactivated)
            throw new EntityConflictException($"User with id {enrollment.UserId} is deactivated.");

        if (enrollmentDTO.Course.IsDeactivated)
            throw new EntityConflictException($"Course with id {enrollment.CourseId} is deactivated.");

        await enrollmentsRepository.GradeStudentAsync(enrollmentDTO, grade);
    }

    public async Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        if (enrollmentDTO.User.IsDeactivated)
            throw new EntityConflictException($"User with id {enrollment.UserId} is deactivated.");

        if (enrollmentDTO.Course.IsDeactivated)
            throw new EntityConflictException($"Course with id {enrollment.CourseId} is deactivated.");

        await enrollmentsRepository.ReviewCourseAsync(enrollmentDTO, review);
    }
}
