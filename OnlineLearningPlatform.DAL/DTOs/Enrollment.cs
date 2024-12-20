﻿
namespace OnlineLearningPlatform.DAL.DTOs;

public class Enrollment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public Guid CourseId { get; set; }

    public Course Course { get; set; }

    public int? Grade { get; set; }

    public int? Attendance { get; set; }

    public string? StudentReview { get; set; }
}
