using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces;

public interface IEnrollmentsRepository
{
    Task ControlAttendanceAsync(Enrollment enrollment, int attendance);

    Task DisenrollAsync(Enrollment enrollment);

    Task EnrollAsync(Enrollment enrollment);

    Task<Enrollment> GetEnrollmentByIdAsync(Guid courseId, Guid userId);

    Task GradeStudentAsync(Enrollment enrollment, int grade);

    Task ReviewCourseAsync(Enrollment enrollment, string review);
}