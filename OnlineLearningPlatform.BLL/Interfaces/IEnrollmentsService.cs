using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces
{
    public interface IEnrollmentsService
    {
        Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance);

        Task DisenrollAsync(EnrollmentManagementModel enrollment);

        Task EnrollAsync(Guid courseId, Guid userId);

        Task<List<CourseEnrollmentModel>> GetEnrollmentsByUserIdAsync(Guid id);

        Task GradeStudentAsync(EnrollmentManagementModel enrollment, int grade);

        Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review);
    }
}