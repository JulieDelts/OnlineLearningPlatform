using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.EntityConfigurations;

internal class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.Property(s => s.Id)
        .IsRequired()
        .ValueGeneratedOnAdd();

        builder.Property(s => s.StudentReview)
        .HasMaxLength(1500);

        builder.HasOne(en => en.User)
        .WithMany(u => u.Enrollments);

        builder.HasOne(en => en.Course)
        .WithMany(c => c.Enrollments);
    }
}
