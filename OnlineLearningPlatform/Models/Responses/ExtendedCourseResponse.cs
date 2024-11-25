namespace OnlineLearningPlatform.Models.Responses;

public class ExtendedCourseResponse: CourseResponse
{
    public Guid Id { get; set; }

    public int NumberOfLessons { get; set; }

    public UserResponse Teacher { get; set; }

    public List<UserResponse> Students { get; set; }
}

