namespace OnlineLearningPlatform.Models.Responses
{
    public class CourseResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NumberOfLessons { get; set; }

        public bool IsStarted { get; set; }

        public UserResponse Teacher { get; set; }
    }
}

