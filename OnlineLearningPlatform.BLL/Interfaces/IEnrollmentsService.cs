using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.BLL.Interfaces
{
    public interface IEnrollmentsService
    {
        Task ControlAttendanceAsync(EnrollmentManagementModel enrollment, int attendance, Guid teacherId);

        Task DisenrollAsync(EnrollmentManagementModel enrollment);

        Task EnrollAsync(Guid courseId, Guid userId);

        Task GradeStudentAsync(EnrollmentManagementModel enrollment, int grade, Guid teacherId);

        Task ReviewCourseAsync(EnrollmentManagementModel enrollment, string review);
    }
}