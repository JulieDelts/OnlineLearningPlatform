namespace OnlineLearningPlatform.Models.Requests
{
    public class EnrollmentRequest
    {
        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }
    }
}
