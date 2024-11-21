namespace OnlineLearningPlatform.Models.Requests
{
    public class CourseReviewRequest
    {
        public Guid UserId { get; set; }

        public string Review {  get; set; }
    }
}
