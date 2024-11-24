namespace OnlineLearningPlatform.Models.Responses
{
    public class ExtendedUserResponse: UserResponse
    {
        public Guid Id { get; set; }

        public string Phone { get; set; }

        public List<CourseResponse>? TaughtCourses { get; set; }

        public List<CourseEnrollmentResponse>? Enrollments { get; set; }
    }
}
