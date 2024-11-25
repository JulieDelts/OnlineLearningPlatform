using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL
{
    public class CoursesService: ICoursesService
    {
        private readonly ICoursesRepository _coursesRepository;

        private readonly IUsersRepository _usersRepository;

        private readonly IMapper _mapper;

        public CoursesService(IUsersRepository usersRepository, ICoursesRepository coursesRepository)
        {
            _usersRepository = usersRepository;

            _coursesRepository = coursesRepository;

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

        public async Task DeactivateCourse(Guid id)
        {
            await _coursesRepository.DeactivateCourse(id);
        }

        public async Task DeleteCourse(Guid id)
        {
            await _coursesRepository.DeleteCourse(id);
        }
    }
}
