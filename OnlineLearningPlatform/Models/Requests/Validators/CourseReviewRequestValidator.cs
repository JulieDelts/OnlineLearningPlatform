using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class CourseReviewRequestValidator: AbstractValidator<CourseReviewRequest>
    {
        public CourseReviewRequestValidator()
        {
            RuleFor(model => model.UserId).NotEmpty().WithMessage("The UserId property must not be empty.");
            RuleFor(model => model.Review).NotEmpty().WithMessage("The FirstName property must not be empty.").Length(1, 1000).WithMessage("The FirstName property must be between 1 and 1000 characters long.");
        }
    }
}
