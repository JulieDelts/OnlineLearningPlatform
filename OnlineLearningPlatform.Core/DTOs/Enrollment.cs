
namespace OnlineLearningPlatform.Core.DTOs
{
    public class Enrollment
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public Course Course { get; set; }
    }
}
