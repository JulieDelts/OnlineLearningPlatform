
namespace OnlineLearningPlatform.BLL.BusinessModels;

public class CreateCourseModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public int NumberOfLessons { get; set; }

    public Guid TeacherId { get; set; }
}
