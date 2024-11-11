namespace OnlineLearningPlatform.Models.Responses
{
    public class UserWithCoursesResponse: UserResponse
    {
        public List<CourseResponse> Courses { get; set; }
    }
}
