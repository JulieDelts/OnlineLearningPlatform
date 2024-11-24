namespace OnlineLearningPlatform.Models.Responses
{
    public class CourseEnrollmentResponse: CourseResponse
    {
        public int? Grade { get; set; }

        public int? Attendance { get; set; }

        public string? StudentReview { get; set; }
    }
}
