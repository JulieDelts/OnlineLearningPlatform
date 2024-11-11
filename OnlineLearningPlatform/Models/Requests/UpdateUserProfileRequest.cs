namespace OnlineLearningPlatform.Models.Requests
{
    public class UpdateUserProfileRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Role Role { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
