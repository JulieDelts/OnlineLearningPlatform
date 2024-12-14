using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces;

public interface ICoursesService
{
    Task<Guid> CreateCourseAsync(CreateCourseModel course);

    Task<List<CourseModel>> GetAllActiveCoursesAsync();

    Task<ExtendedCourseModel> GetFullCourseByIdAsync(Guid id);

    Task UpdateCourseAsync(Guid id, UpdateCourseModel course, Guid teacherId);
    
    Task DeactivateCourseAsync(Guid id, Guid teacherId);

    Task DeleteCourseAsync(Guid id);

    Task<List<UserEnrollmentModel>> GetEnrollmentsByCourseIdAsync(Guid id);
}
