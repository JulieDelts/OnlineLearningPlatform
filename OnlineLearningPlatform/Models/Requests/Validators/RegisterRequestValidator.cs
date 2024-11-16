using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class RegisterRequestValidator: AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator() 
        {
            RuleFor(model => model.FirstName).NotEmpty().WithMessage("The FirstName property must not be empty.").Length(1, 30).WithMessage("The FirstName property must be between 1 and 30 characters long.");
            RuleFor(model => model.LastName).NotEmpty().WithMessage("The LastName property must not be empty.").Length(1, 30).WithMessage("The LastName property must be between 1 and 30 characters long.");
            RuleFor(model => model.Login).NotEmpty().WithMessage("The Login property must not be empty.").Length(5, 10).WithMessage("The Login property must be between 5 and 10 characters long.");
            RuleFor(model => model.Password).NotEmpty().WithMessage("The Password property must not be empty.").Length(8, 15).WithMessage("The Password property must be between 8 and 15 characters long.");
            RuleFor(model => model.Email).NotEmpty().WithMessage("The Email property must not be empty.").EmailAddress().WithMessage("The Email property must be a valid email address.");
            RuleFor(model => model.Phone).Length(10, 15).NotEmpty().WithMessage("The Phone property must not be empty.").WithMessage("The Phone property must be a valid phone number.");
        }
    }
}
