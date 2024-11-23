using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.DAL.DTOs
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public Role Role { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool IsDeactivated { get; set; }

        public ICollection<Course>? TaughtCourses { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
