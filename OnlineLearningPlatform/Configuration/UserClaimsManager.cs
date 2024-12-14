using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.Core;

namespace OnlineLearningPlatform.Configuration
{
    public static class UserClaimsManager
    {
        public static Guid GetUserIdFromClaims(this ControllerBase controller)
        {
            var user = controller.HttpContext.User;

            var userIdClaim = user.FindFirst("SystemId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new ClaimsRetrievalException("The user id claim is not provided.");

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                throw new ClaimsRetrievalException("The id format is not supported.");

            return userId;
        }

        public static Role GetUserRoleFromClaims(this ControllerBase controller)
        {
            var user = controller.HttpContext.User;

            var userRoleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRoleClaim))
                throw new ClaimsRetrievalException("The user role claim is not provided.");

            if (!Enum.TryParse(userRoleClaim, out Role role))
                throw new ClaimsRetrievalException("The role is not defined.");
            else
                return role;
        }
    }
}
