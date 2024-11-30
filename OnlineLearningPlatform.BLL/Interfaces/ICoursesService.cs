using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces;

public interface ICoursesService
{
    Task<Guid> CreateCourseAsync(CreateCourseModel course);

    Task<List<CourseModel>> GetAllCoursesAsync();

    Task<ExtendedCourseModel> GetCourseByIdAsync(Guid id);

    Task UpdateCourseAsync(Guid id, UpdateCourseModel course);

    Task DeactivateCourseAsync(Guid id);

    Task DeleteCourseAsync(Guid id);

    Task EnrollAsync(Guid courseId, Guid userId);

    Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review);

    Task GradeStudentAsync(EnrollmentManagementModel enrollment, int grade);

    Task DisenrollAsync(EnrollmentManagementModel enrollment);

    Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance);
}
