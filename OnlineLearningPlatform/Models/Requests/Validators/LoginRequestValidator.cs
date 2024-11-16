using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class LoginRequestValidator: AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(model => model.Login).NotEmpty().WithMessage("The Login property must not be empty.").Length(5, 10).WithMessage("The Login property must be between 5 and 10 characters long.");
            RuleFor(model => model.Password).NotEmpty().WithMessage("The Password property must not be empty.").Length(8, 15).WithMessage("The Password property must be between 8 and 15 characters long.");
        }
    }
}
