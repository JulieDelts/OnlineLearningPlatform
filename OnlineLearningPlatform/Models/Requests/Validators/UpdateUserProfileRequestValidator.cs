using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class UpdateUserProfileRequestValidator: AbstractValidator<UpdateUserProfileRequest>
    {
        public UpdateUserProfileRequestValidator()
        {
            RuleFor(model => model.FirstName).NotEmpty().WithMessage("The FirstName property must not be empty.").Length(1, 30).WithMessage("The FirstName property must be between 1 and 30 characters long.");
            RuleFor(model => model.LastName).NotEmpty().WithMessage("The LastName property must not be empty.").Length(1, 30).WithMessage("The LastName property must be between 1 and 30 characters long.");
            RuleFor(model => model.Email).NotEmpty().WithMessage("The Email property must not be empty.").EmailAddress().WithMessage("The Email property must be a valid email address.");
            RuleFor(model => model.Phone).NotEmpty().WithMessage("The Phone property must not be empty.").Length(10, 15).WithMessage("The Phone property must be a valid phone number.");
        }
    }
}
