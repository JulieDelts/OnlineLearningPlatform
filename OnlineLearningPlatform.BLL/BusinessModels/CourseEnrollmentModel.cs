
namespace OnlineLearningPlatform.BLL.BusinessModels;

public class CourseEnrollmentModel: CourseModel
{
    public int? Grade { get; set; }

    public int? Attendance { get; set; }

    public string? StudentReview { get; set; }
}
