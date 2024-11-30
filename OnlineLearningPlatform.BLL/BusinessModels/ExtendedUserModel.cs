
using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.BLL.BusinessModels;

public class ExtendedUserModel: UserModel
{
    public Guid Id { get; set; }

    public Role Role { get; set; }

    public string Phone { get; set; }

    public List<CourseModel> TaughtCourses { get; set; }

    public List<CourseEnrollmentModel> Enrollments { get; set; }
}
