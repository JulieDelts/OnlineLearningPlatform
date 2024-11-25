namespace OnlineLearningPlatform.Models.Requests;

public class UpdateCourseRequest
{
    public string Name { get; set; }

    public string Description { get; set; }

    public int NumberOfLessons { get; set; }
}
