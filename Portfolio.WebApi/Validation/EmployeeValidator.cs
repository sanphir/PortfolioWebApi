using FluentValidation;
using Portfolio.DAL.Models;
using Portfolio.WebApi.Helpers;

namespace Portfolio.WebApi.Validation
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be specified").MaximumLength(50).WithMessage("Name length must be less than 50 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must be specified").Must(x => x?.IsEmail() ?? false).WithMessage("Not a valid email").MaximumLength(50).WithMessage("Email length must be less than 50 characters");
            RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zerro");
            RuleFor(x => x.BirthDate).LessThan(DateTime.Now.AddYears(-18)).WithMessage("Employee must be over 18 years of age");
        }
    }
}
