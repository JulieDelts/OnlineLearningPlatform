using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces
{
    public interface ICoursesRepository
    {
        Task ControlAttendance(Enrollment enrollment);

        Task<Guid> CreateCourse(Course course);

        Task DeactivateCourse(Guid id);

        Task DeleteCourse(Guid id);

        Task Disenroll(Enrollment enrollment);

        Task Enroll(Enrollment enrollment);

        Task<List<Course>> GetAllCourses();

        Task<Course> GetCourseByIdWithFullInfo(Guid id);

        Task<Course> GetCourseById(Guid id);

        Task GradeStudent(Enrollment enrollment);

        Task ReviewCourse(Enrollment enrollment);

        Task UpdateCourse(Guid id, Course course);
    }
}