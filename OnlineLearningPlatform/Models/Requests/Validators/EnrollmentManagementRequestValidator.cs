using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class EnrollmentManagementRequestValidator: AbstractValidator<EnrollmentManagementRequest>
    {
        public EnrollmentManagementRequestValidator()
        {
            RuleFor(model => model.UserId).NotEmpty().WithMessage("The UserId property must not be empty.");
        }
    }
}
