namespace OnlineLearningPlatform.Models.Requests;

public class GradeStudentRequest
{
    public Guid UserId { get; set; }

    public int Grade {  get; set; } 
}
