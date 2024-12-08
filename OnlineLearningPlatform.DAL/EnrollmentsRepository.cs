using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL;

public class EnrollmentsRepository(OnlineLearningPlatformContext context) : IEnrollmentsRepository
{
    public async Task EnrollAsync(Enrollment enrollment)
    {
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(Guid courseId, Guid userId) =>
        await context.Enrollments
        .Include(en => en.Course)
        .Include(en => en.User)
        .SingleOrDefaultAsync(en => en.User.Id == userId && en.Course.Id == courseId);


    public async Task ReviewCourseAsync(Enrollment enrollment, string review)
    {
        enrollment.StudentReview = review;
        await context.SaveChangesAsync();
    }

    public async Task GradeStudentAsync(Enrollment enrollment, int grade)
    {
        enrollment.Grade = grade;
        await context.SaveChangesAsync();
    }

    public async Task ControlAttendanceAsync(Enrollment enrollment, int attendance)
    {
        enrollment.Attendance = attendance;
        await context.SaveChangesAsync();
    }

    public async Task DisenrollAsync(Enrollment enrollment)
    {
        context.Enrollments.Remove(enrollment);
        await context.SaveChangesAsync();
    }
}
