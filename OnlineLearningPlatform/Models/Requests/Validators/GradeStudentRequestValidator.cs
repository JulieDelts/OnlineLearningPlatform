using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class GradeStudentRequestValidator: AbstractValidator<GradeStudentRequest>
    {
        public GradeStudentRequestValidator() 
        {
            RuleFor(model => model.UserId).NotEmpty().WithMessage("The UserId property must not be empty.");
            RuleFor(model => model.Grade).NotEmpty().WithMessage("The Grade property must not be empty.").InclusiveBetween(0, 10).WithMessage("The Grade property must be between 1 and 10.");
        }
    }
}
