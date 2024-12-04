using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces;

public interface ICoursesService
{
    Task<Guid> CreateCourseAsync(CreateCourseModel course);

    Task<List<CourseModel>> GetAllActiveCoursesAsync();

    Task<ExtendedCourseModel> GetCourseByIdAsync(Guid id);

    Task UpdateCourseAsync(Guid id, UpdateCourseModel course);

    Task DeactivateCourseAsync(Guid id);

    Task DeleteCourseAsync(Guid id);

    Task<List<UserEnrollmentModel>> GetEnrollmentsByCourseIdAsync(Guid id);
}
