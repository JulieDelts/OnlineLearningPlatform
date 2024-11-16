﻿using FluentValidation;

namespace OnlineLearningPlatform.Models.Requests.Validators
{
    public class UpdateCourseRequestValidator: AbstractValidator<UpdateCourseRequest>
    {
        public UpdateCourseRequestValidator()
        {
            RuleFor(model => model.Name).NotEmpty().WithMessage("The Name property must not be empty.").Length(1, 100).WithMessage("The Name property must be between 1 and 100 characters long.");
            RuleFor(model => model.Description).Length(1, 1000).WithMessage("The Description property must be between 1 and 1000 characters long.");
            RuleFor(model => model.NumberOfLessons).NotEmpty().WithMessage("The NumberOfLessons property must not be empty.").GreaterThan(1).WithMessage("The NumberOfLessons property must be greater than 1.");
        }
    }
}
