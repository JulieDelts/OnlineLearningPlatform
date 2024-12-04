namespace OnlineLearningPlatform.Models.Responses;

public class UserEnrollmentResponse: UserResponse
{
    public int? Grade { get; set; }

    public int? Attendance { get; set; }
}
