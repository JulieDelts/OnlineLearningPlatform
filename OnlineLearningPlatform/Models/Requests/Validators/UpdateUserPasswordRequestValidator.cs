using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class UpdateUserPasswordRequestValidator: AbstractValidator<UpdateUserPasswordRequest>
    {
        public UpdateUserPasswordRequestValidator() 
        {
            RuleFor(model => model.CurrentPassword).NotEmpty().WithMessage("The Password property must not be empty.").Length(8, 15).WithMessage("The Password property must be between 8 and 15 characters long.");
            RuleFor(model => model.NewPassword).NotEmpty().WithMessage("The Password property must not be empty.").Length(8, 15).WithMessage("The Password property must be between 8 and 15 characters long.");
        }
    }
}
