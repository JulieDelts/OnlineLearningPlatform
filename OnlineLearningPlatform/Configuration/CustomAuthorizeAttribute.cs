using Microsoft.AspNetCore.Authorization;
using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.Configuration;

public class CustomAuthorizeAttribute: AuthorizeAttribute
{
    public CustomAuthorizeAttribute(Role[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
