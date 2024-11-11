﻿namespace OnlineLearningPlatform.Models.Requests
{
    public class CreateCourseRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NumberOfLessons { get; set; }

        public Guid TeacherId { get; set; }
    }
}
