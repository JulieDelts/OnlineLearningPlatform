namespace OnlineLearningPlatform.Models.Requests;

public class ControlAttendanceRequest
{
    public Guid UserId { get; set; }

    public int Attendance { get; set; }
}
