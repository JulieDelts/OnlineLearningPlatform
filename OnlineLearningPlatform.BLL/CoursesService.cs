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

    public async Task<Guid> CreateCourse(CreateCourseModel course)
    {
        var courseDTO = _mapper.Map<Course>(course);

        var user = await _usersRepository.GetUserById(course.TeacherId);

        if (user.Role != Role.Teacher)
        {
            throw new ArgumentException();
        }

        courseDTO.Teacher = user;

        var id = await _coursesRepository.CreateCourse(courseDTO);

        return id;
    }

    public async Task<List<CourseModel>> GetAllCourses()
    { 
        var courseDTOs = await _coursesRepository.GetAllCourses();

        var courses = _mapper.Map<List<CourseModel>>(courseDTOs);

        return courses;
    }

    public async Task<ExtendedCourseModel> GetCourseById(Guid id)
    { 
        var courseDTO = await _coursesRepository.GetCourseByIdWithFullInfo(id);

        var course = _mapper.Map<ExtendedCourseModel>(courseDTO);

        var students = _mapper.Map<List<UserModel>>(courseDTO.Enrollments.Select(e => e.User));

        course.Students = students;
        
        return course;
    }

    public async Task UpdateCourse(Guid id, UpdateCourseModel course)
    {
        var courseDTO = _mapper.Map<Course>(course);

        await _coursesRepository.UpdateCourse(id,courseDTO);
    }

    public async Task DeactivateCourse(Guid id) => await _coursesRepository.DeactivateCourse(id);

    public async Task DeleteCourse(Guid id)
    {
        var course = _coursesRepository.GetCourseById(id);
        if (course == null)
            throw new EntityNotFoundException($"Course with id {id} was not found");  // ex.Message

        await _coursesRepository.DeleteCourse(id);
    }

    public async Task EnrollStudent(Guid courseId, Guid userId)
    {
        var course = await _coursesRepository.GetCourseById(courseId);
        if (course == null)
            throw new EntityNotFoundException($"Course with id {courseId} was not found");

        var user = await _usersRepository.GetUserById(userId); 
        if (user == null)
            throw new EntityNotFoundException($"User with id {userId} was not found");

        var newEnrollment = new Enrollment()
        {
            Course = course,
            User = user
        };

        await _enrollmentsRepository.CreateEnrollmentAsync(newEnrollment);
    }
}
