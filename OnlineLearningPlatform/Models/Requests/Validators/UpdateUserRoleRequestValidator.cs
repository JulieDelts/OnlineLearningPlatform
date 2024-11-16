using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class UpdateUserRoleRequestValidator: AbstractValidator<UpdateUserRoleRequest>
    {
        public UpdateUserRoleRequestValidator()
        {
            RuleFor(model => model.Role).IsInEnum().WithMessage("The Role property must be a valid role in the system.");
        }
    }
}
