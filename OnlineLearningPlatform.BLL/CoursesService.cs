using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class CoursesService: ICoursesService
{
    private readonly ICoursesRepository _coursesRepository;
    private readonly IEnrollmentsRepository _enrollmentsRepository;
    private readonly IUsersRepository _usersRepository;

    private readonly IMapper _mapper;

    public CoursesService(
        IUsersRepository usersRepository,
        ICoursesRepository coursesRepository,
        IEnrollmentsRepository enrollmentsRepository
    )
    {
        _usersRepository = usersRepository;
        _coursesRepository = coursesRepository;
        _enrollmentsRepository = enrollmentsRepository;

        var config = new MapperConfiguration(
           cfg =>
           {
               cfg.AddProfile(new BLLUserMapperProfile());
               cfg.AddProfile(new BLLCourseMapperProfile());
           });
        _mapper = new Mapper(config);
    }

    public async Task<Guid> CreateCourseAsync(CreateCourseModel course)
    {
        var courseDTO = _mapper.Map<Course>(course);

        var user = await _usersRepository.GetUserByIdAsync(course.TeacherId);

        if (user.Role != Role.Teacher)
            throw new EntityConflictException("The role of the user is not correct.");

        courseDTO.Teacher = user;

        var id = await _coursesRepository.CreateCourseAsync(courseDTO);

        return id;
    }

    public async Task<List<CourseModel>> GetAllCoursesAsync()
    { 
        var courseDTOs = await _coursesRepository.GetAllCoursesAsync();

        var courses = _mapper.Map<List<CourseModel>>(courseDTOs);

        return courses;
    }

    public async Task<ExtendedCourseModel> GetCourseByIdAsync(Guid id)
    { 
        var courseDTO = await _coursesRepository.GetCourseByIdWithFullInfoAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var course = _mapper.Map<ExtendedCourseModel>(courseDTO);

        var students = _mapper.Map<List<UserModel>>(courseDTO.Enrollments.Select(e => e.User));

        course.Students = students;
        
        return course;
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseModel course)
    {
        var courseDTO = await _coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var courseUpdate = _mapper.Map<Course>(course);

        await _coursesRepository.UpdateCourseAsync(courseDTO, courseUpdate);
    }

    public async Task DeactivateCourseAsync(Guid id)
    {
        var courseDTO = await _coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        await _coursesRepository.DeactivateCourseAsync(courseDTO);
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var courseDTO = await _coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        await _coursesRepository.DeleteCourseAsync(courseDTO);
    }

    public async Task EnrollAsync(Guid courseId, Guid userId)
    {
        var courseDTO = await _coursesRepository.GetCourseByIdAsync(courseId);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {courseId} was not found.");

        var userDTO = await _usersRepository.GetUserByIdAsync(userId); 

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {userId} was not found.");

        var enrollmentDTO = await _enrollmentsRepository.GetEnrollmentByIdAsync(courseId, userId);

        if (enrollmentDTO != null)
            throw new EntityConflictException($"Enrollment with user id {userId} and course id {courseId} already exists.");

        var newEnrollment = new Enrollment()
        {
            Course = courseDTO,
            User = userDTO
        };

        await _enrollmentsRepository.EnrollAsync(newEnrollment);
    }

    public async Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance)
    {
        var enrollmentDTO = await _enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        await _enrollmentsRepository.ControlAttendanceAsync(enrollmentDTO, attendance);
    }

    public async Task DisenrollAsync(EnrollmentManagementModel enrollment)
    {
        var enrollmentDTO = await _enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        await _enrollmentsRepository.DisenrollAsync(enrollmentDTO);
    }

    public async Task GradeStudentAsync(EnrollmentManagementModel enrollment, int grade)
    {
        var enrollmentDTO = await _enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        await _enrollmentsRepository.GradeStudentAsync(enrollmentDTO, grade);
    }

    public async Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review)
    {
        var enrollmentDTO = await _enrollmentsRepository.GetEnrollmentByIdAsync(enrollment.CourseId, enrollment.UserId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {enrollment.UserId} and course id {enrollment.CourseId} was not found.");

        await _enrollmentsRepository.ReviewCourseAsync(enrollmentDTO, review);
    }
}
