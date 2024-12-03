using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.Models.Responses;

public class ExtendedUserResponse: UserResponse
{
    public Guid Id { get; set; }

    public Role Role { get; set; }

    public string Phone { get; set; }
}
