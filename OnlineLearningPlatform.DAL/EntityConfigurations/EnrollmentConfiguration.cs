using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.EntityConfigurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.Property(s => s.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

            builder.Property(s => s.Grade)
            .IsRequired();

            builder.Property(s => s.Attendance)
            .IsRequired();

            builder.Property(s => s.StudentReview)
            .IsRequired()
            .HasMaxLength(1500);

            builder.HasOne(en => en.User)
            .WithMany(u => u.Enrollments);

            builder.HasOne(en => en.Course)
            .WithMany(c => c.Enrollments);
        }
    }
}
