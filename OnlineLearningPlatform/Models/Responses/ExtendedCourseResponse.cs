namespace OnlineLearningPlatform.Models.Responses
{
    public class ExtendedCourseResponse: CourseResponse
    {
        public Guid Id { get; set; }

        public int NumberOfLessons { get; set; }

        public bool IsStarted { get; set; }

        public List<UserResponse> Users { get; set; }
    }
}

