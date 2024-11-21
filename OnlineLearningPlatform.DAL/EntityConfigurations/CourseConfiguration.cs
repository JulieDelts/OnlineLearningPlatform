using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.EntityConfigurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasIndex(u => u.Name)
            .IsUnique();

            builder.Property(s => s.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(s => s.Description)
            .HasMaxLength(1000);

            builder.Property(s => s.NumberOfLessons)
            .IsRequired();

            builder.Property(s => s.IsDeactivated)
            .IsRequired()
            .HasDefaultValue(false);

            builder.HasOne(c => c.Teacher)
            .WithMany(u => u.TaughtCourses);
        }
    }
}
