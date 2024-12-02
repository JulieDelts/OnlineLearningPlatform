using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.DAL.Configuration.EntityConfigurations;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL;

public class OnlineLearningPlatformContext(DbContextOptions<OnlineLearningPlatformContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
    }
}
