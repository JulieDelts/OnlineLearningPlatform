using System.Data;

namespace OnlineLearningPlatform.Models.Requests
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public Role Role { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
