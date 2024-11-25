using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces
{
    public interface ICoursesService
    {
        Task<Guid> CreateCourse(CreateCourseModel course);

        Task<List<CourseModel>> GetAllCourses();

        Task<ExtendedCourseModel> GetCourseById(Guid id);

        Task UpdateCourse(Guid id, UpdateCourseModel course);

        Task DeactivateCourse(Guid id);

        Task DeleteCourse(Guid id);
    }
}
