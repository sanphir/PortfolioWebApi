using FluentValidation;
using Portfolio.DAL.Models;

namespace Portfolio.WebApi.Validation
{
    public class WorkTaskValidator : AbstractValidator<WorkTask>
    {
        public WorkTaskValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title must be specified");
            RuleFor(x => x.DueDate).GreaterThan(DateTime.Now).WithMessage("Due date must be greater then now");
        }
    }
}
