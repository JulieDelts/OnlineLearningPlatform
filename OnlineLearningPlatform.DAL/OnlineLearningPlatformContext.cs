using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL
{
    public class OnlineLearningPlatformContext: DbContext
    {
        public DbSet<User> User { get; set; }

        public DbSet<Course> Course { get; set; }

        public DbSet<Enrollment> Enrollment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Host=localhost;Port=5432;Database=OLPDB;Username=postgres;Password=postgres;";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
