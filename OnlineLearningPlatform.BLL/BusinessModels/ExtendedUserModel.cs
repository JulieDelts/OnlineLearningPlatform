
using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.BLL.BusinessModels;

public class ExtendedUserModel: UserModel
{
    public Guid Id { get; set; }

    public Role Role { get; set; }

    public string Phone { get; set; }

    public bool IsDeactivated { get; set; }
}
