
namespace OnlineLearningPlatform.DAL.DTOs;

public class Course
{
    public Guid Id { get; set; } 

    public string Name { get; set; }

    public string Description { get; set; }

    public int NumberOfLessons { get; set; }

    public bool IsDeactivated { get; set; }

    public Guid TeacherId { get; set; }

    public User Teacher { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = [];
}
