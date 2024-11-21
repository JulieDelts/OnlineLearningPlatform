using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class ControlAttendanceRequestValidator: AbstractValidator<ControlAttendanceRequest>
    {
       public ControlAttendanceRequestValidator()
       {
            RuleFor(model => model.UserId).NotEmpty().WithMessage("The UserId property must not be empty.");
            RuleFor(model => model.Attendance).NotEmpty().WithMessage("The Attendance property must not be empty.").GreaterThanOrEqualTo(0).WithMessage("The Attendance property must be greater than or equal to 0.");
        }
    }
}
