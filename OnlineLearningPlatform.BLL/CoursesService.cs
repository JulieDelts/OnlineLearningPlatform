using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class CoursesService(
    IUsersRepository usersRepository,
    ICoursesRepository coursesRepository,
    IEnrollmentsRepository enrollmentsRepository,
    IMapper mapper
    ) : ICoursesService
{
    public async Task<Guid> CreateCourseAsync(CreateCourseModel course)
    {
        var courseDTO = mapper.Map<Course>(course);

        var user = await usersRepository.GetUserByIdAsync(course.TeacherId);

        if (user.Role != Role.Teacher)
            throw new EntityConflictException("The role of the user is not correct.");

        courseDTO.Teacher = user;

        var id = await coursesRepository.CreateCourseAsync(courseDTO);

        return id;
    }

    public async Task<List<CourseModel>> GetAllCoursesAsync()
    { 
        var courseDTOs = await coursesRepository.GetAllCoursesAsync();

        var courses = mapper.Map<List<CourseModel>>(courseDTOs);

        return courses;
    }

    public async Task<ExtendedCourseModel> GetCourseByIdAsync(Guid id)
    { 
        var courseDTO = await coursesRepository.GetCourseByIdWithFullInfoAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var course = mapper.Map<ExtendedCourseModel>(courseDTO);

        var students = mapper.Map<List<UserModel>>(courseDTO.Enrollments.Select(e => e.User));

        course.Students = students;
        
        return course;
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseModel course)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var courseUpdate = mapper.Map<Course>(course);

        await coursesRepository.UpdateCourseAsync(courseDTO, courseUpdate);
    }

    public async Task DeactivateCourseAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        await coursesRepository.DeactivateCourseAsync(courseDTO);
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        await coursesRepository.DeleteCourseAsync(courseDTO);
    }

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
