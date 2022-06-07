using FluentValidation;
using Portfolio.DAL.Models;

namespace Portfolio.WebApi.Validation
{
    public class WorkTaskValidator : AbstractValidator<WorkTask>
    {
        public WorkTaskValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title must be specified").MaximumLength(50).WithMessage("Title length must be less than 50 characters");
            RuleFor(x => x.Content).MaximumLength(500).WithMessage("Conten length must be less than 500 characters");
            //validating on front end
            //RuleFor(x => x.DueDate).GreaterThan(DateTime.Now).WithMessage("Due date must be greater then now");
        }
    }
}
