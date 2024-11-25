using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL;

public class EnrollmentsRepository : IEnrollmentsRepository
{
    private readonly OnlineLearningPlatformContext _context;

    public EnrollmentsRepository(OnlineLearningPlatformContext context)
    {
        _context = context;
    }

    public async Task EnrollAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<Enrollment> GetEnrollmentByIdAsync(Guid courseId, Guid userId)
    {
        return await _context.Enrollments.Where(en => en.User.Id == userId && en.Course.Id == courseId).SingleOrDefaultAsync();
    }

    public async Task ReviewCourseAsync(Enrollment enrollment, string review)
    {
        enrollment.StudentReview = review;
        await _context.SaveChangesAsync();
    }

    public async Task GradeStudentAsync(Enrollment enrollment, int grade)
    {
        enrollment.Grade = grade;
        await _context.SaveChangesAsync();
    }

    public async Task ControlAttendanceAsync(Enrollment enrollment, int attendance)
    {
        enrollment.Attendance = attendance;
        await _context.SaveChangesAsync();
    }

    public async Task DisenrollAsync(Enrollment enrollment)
    {
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
    }
}
