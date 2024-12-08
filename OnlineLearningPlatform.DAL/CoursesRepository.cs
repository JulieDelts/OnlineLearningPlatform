using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL;

public class CoursesRepository(OnlineLearningPlatformContext context) : ICoursesRepository
{
    public async Task<Guid> CreateCourseAsync(Course course)
    {
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        return course.Id;
    }

    public async Task<List<Course>> GetAllActiveCoursesAsync() => await context.Courses.Where(c => c.IsDeactivated == false).ToListAsync();

    public async Task<Course?> GetCourseByIdWithFullInfoAsync(Guid id) =>
        await context.Courses
        .Include(u => u.Enrollments)
            .ThenInclude(en => en.User)
        .Include(u => u.Teacher)
        .SingleOrDefaultAsync(c => c.Id == id);
    

    public async Task<Course?> GetCourseByIdAsync(Guid id) => await context.Courses.SingleOrDefaultAsync(c => c.Id == id);

    public async Task UpdateCourseAsync(Course course, Course courseUpdate)
    {
        course.Name = courseUpdate.Name;
        course.Description = courseUpdate.Description;
        course.NumberOfLessons = courseUpdate.NumberOfLessons;
        await context.SaveChangesAsync();
    }

    public async Task DeactivateCourseAsync(Course course)
    {
        course.IsDeactivated = true;
        await context.SaveChangesAsync();
    }

    public async Task DeleteCourseAsync(Course course)
    {
        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }
}
