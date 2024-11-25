using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.Models.Responses;

public class UserResponse
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Role Role { get; set; }

    public string Email { get; set; }
}
