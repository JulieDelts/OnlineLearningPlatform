using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Login)
            .IsUnique();

            builder.Property(s => s.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

            builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(30);

            builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(30);

            builder.Property(s => s.Login)
            .IsRequired()
            .HasMaxLength(10);

            builder.Property(s => s.Password)
            .IsRequired()
            .HasMaxLength(15);

            builder.Property(s => s.Role)
            .IsRequired();

            builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(15);

            builder.Property(s => s.IsDeactivated)
            .IsRequired()
            .HasDefaultValue(false);
        }
    }
}
