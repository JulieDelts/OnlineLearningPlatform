using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL
{
    public class CoursesRepository : ICoursesRepository
    {
        private OnlineLearningPlatformContext _context;

        public CoursesRepository()
        {
            _context = new OnlineLearningPlatformContext();
        }

        public async Task<Guid> CreateCourse(Course course)
        {
            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return course.Id;
        }

        public async Task Enroll(Enrollment enrollment)
        {
            var course = _context.Course.Where(c => c.Id == enrollment.Course.Id).FirstOrDefault();
            var user = _context.User.Where(u => u.Id == enrollment.User.Id).FirstOrDefault();

            if (course != null && user != null)
            {
                enrollment.Course = course;
                enrollment.User = user;
                _context.Enrollment.Add(enrollment);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entities are not found.");
            }
        }

        public async Task<List<Course>> GetAllCourses()
        {
            var courses = await _context.Course.Where(c => c.IsDeactivated == false).ToListAsync();

            if (courses != null)
            {
                return courses;
            }
            else
            {
                return new List<Course>();
            }
        }

        public async Task<Course> GetCourseByIdWithFullInfo(Guid id)
        {
            var course = await _context.Course.Where(c => c.Id == id).Include(u => u.Enrollments).ThenInclude(en => en.User).Include(u => u.Teacher).FirstOrDefaultAsync();

            if (course != null)
            {
                return course;
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task UpdateCourse(Guid id, Course course)
        {
            var courseToUpdate = await GetCourseById(id);

            courseToUpdate.Name = course.Name;
            courseToUpdate.Description = course.Description;
            courseToUpdate.NumberOfLessons = course.NumberOfLessons;
            await _context.SaveChangesAsync();
        }

        public async Task ReviewCourse(Enrollment enrollment)
        {
            var enrollmentToUpdate = await GetEnrollmentById(enrollment);

            enrollmentToUpdate.StudentReview = enrollment.StudentReview;
            await _context.SaveChangesAsync();
        }

        public async Task GradeStudent(Enrollment enrollment)
        {
            var enrollmentToUpdate = await GetEnrollmentById(enrollment);

            enrollmentToUpdate.Grade = enrollment.Grade;
            await _context.SaveChangesAsync();
        }

        public async Task ControlAttendance(Enrollment enrollment)
        {
            var enrollmentToUpdate = await GetEnrollmentById(enrollment);

            enrollmentToUpdate.Attendance = enrollment.Attendance;
            await _context.SaveChangesAsync();
        }

        public async Task Disenroll(Enrollment enrollment)
        {
            var enrollmentToDeactivate = await GetEnrollmentById(enrollment);

            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateCourse(Guid id)
        {
            var courseToUpdate = await GetCourseById(id);

            courseToUpdate.IsDeactivated = true;
            await _context.SaveChangesAsync();

        }

        public async Task DeleteCourse(Guid id)
        {
            var courseToDelete = await GetCourseById(id);

            _context.Course.Remove(courseToDelete);
            await _context.SaveChangesAsync();
        }

        private async Task<Course> GetCourseById(Guid id)
        {
            var course = await _context.Course.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (course != null)
            {
                return course;
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        private async Task<Enrollment> GetEnrollmentById(Enrollment enrollment)
        {
            var enrollmentToGet = await _context.Enrollment.Where(en => en.User.Id == enrollment.User.Id && en.Course.Id == enrollment.Course.Id).FirstOrDefaultAsync();

            if (enrollmentToGet != null)
            {
                return enrollmentToGet;
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }
    }
}
