
namespace OnlineLearningPlatform.BLL.BusinessModels;

public class ExtendedCourseModel: CourseModel
{
    public Guid Id { get; set; }

    public int NumberOfLessons { get; set; }

    public UserModel Teacher { get; set; }

    public bool IsDeactivated { get; set; }
}
