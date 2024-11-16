namespace OnlineLearningPlatform.Models.Responses
{
    public class ExtendedUserResponse: UserResponse
    {
        public Guid Id { get; set; }

        public string Phone { get; set; }

        public List<CourseResponse> Courses { get; set; }
    }
}
