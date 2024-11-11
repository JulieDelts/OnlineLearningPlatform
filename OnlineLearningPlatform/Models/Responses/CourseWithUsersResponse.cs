namespace OnlineLearningPlatform.Models.Responses
{
    public class CourseWithUsersResponse: CourseResponse
    {
        public List<UserResponse> Users { get; set; }
    }
}
