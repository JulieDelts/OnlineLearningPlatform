namespace OnlineLearningPlatform.Models.Requests
{
    public class UpdateUserPasswordRequest
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
