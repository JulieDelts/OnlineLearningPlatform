using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using AutoMapper;
using OnlineLearningPlatform.DAL.Interfaces;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.BLL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class EnrollmentsService(
    IUsersRepository usersRepository,
    ICoursesRepository coursesRepository,
    IEnrollmentsRepository enrollmentsRepository,
    IMapper mapper
    ) : IEnrollmentsService
{
    public async Task EnrollAsync(Guid courseId, Guid userId)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(courseId);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {courseId} was not found.");

        var userDTO = await usersRepository.GetUserByIdAsync(userId);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {userId} was not found.");

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

    public async Task<List<CourseEnrollmentModel>> GetEnrollmentsByUserIdAsync(Guid id)
    {
        var user = await usersRepository.GetUserByIdWithFullInfoAsync(id);

        if (user == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        if (user.Role != Role.Student)
            throw new EntityConflictException("The role of the user is not correct.");

        var enrollments = mapper.Map<List<CourseEnrollmentModel>>(user.Enrollments);

        return enrollments;
    }

    public async Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

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

        await enrollmentsRepository.GradeStudentAsync(enrollmentDTO, grade);
    }

    public async Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        await enrollmentsRepository.ReviewCourseAsync(enrollmentDTO, review);
    }
}
