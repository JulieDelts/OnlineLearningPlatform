using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Core.DTOs;

namespace OnlineLearningPlatform.DAL
{
    public class CoursesRepository
    {
        private OnlineLearningPlatformContext _context;

        public CoursesRepository()
        {
            _context = new OnlineLearningPlatformContext();
        }

        public async Task<Guid> Create(Course course)
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

        public async Task<List<Course>> GetAll()
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

        public async Task<Course> GetById(Guid id)
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

        public async Task Update(Guid id, Course course)
        {
            var courseToUpdate = await _context.Course.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (courseToUpdate != null)
            {
                courseToUpdate.Name = course.Name;
                courseToUpdate.Description = course.Description;
                courseToUpdate.NumberOfLessons = course.NumberOfLessons;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task Disenroll(Enrollment enrollment)
        {
            var enrollmentToDeactivate = _context.Enrollment.Where(en => en.User.Id == enrollment.User.Id && en.Course.Id == enrollment.Course.Id).FirstOrDefaultAsync();

            if (enrollmentToDeactivate != null)
            {
                _context.Enrollment.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entities are not found.");
            }
        }

        public async Task Deactivate(Guid id)
        {
            var courseToUpdate = await _context.Course.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (courseToUpdate != null)
            {
                courseToUpdate.IsDeactivated = true;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task Delete(Guid id)
        {
            var courseToDelete = await _context.Course.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (courseToDelete != null)
            {
                _context.Course.Remove(courseToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }
    }
}
