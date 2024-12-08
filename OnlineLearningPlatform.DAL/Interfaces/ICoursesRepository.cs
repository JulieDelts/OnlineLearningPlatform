using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces;

public interface ICoursesRepository
{
    Task<Guid> CreateCourseAsync(Course course);

    Task DeactivateCourseAsync(Course course);

    Task DeleteCourseAsync(Course course);

    Task<List<Course>> GetAllActiveCoursesAsync();

    Task<Course?> GetCourseByIdWithFullInfoAsync(Guid id);

    Task<Course?> GetCourseByIdAsync(Guid id);

    Task UpdateCourseAsync(Course course, Course courseUpdate);
}