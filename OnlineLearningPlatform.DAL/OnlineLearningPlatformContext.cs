using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.EntityConfigurations;

namespace OnlineLearningPlatform.DAL;

public class OnlineLearningPlatformContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigOptions.ConnectionString;
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
    }
}
