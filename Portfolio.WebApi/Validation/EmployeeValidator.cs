using FluentValidation;
using Portfolio.DAL.Models;
using Portfolio.WebApi.Helpers;

namespace Portfolio.WebApi.Validation
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be specified");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must be specified").Must(x => x?.IsEmail() ?? false).WithMessage("Not a valid email");
            RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zerro");
            RuleFor(x => x.BirthDate).LessThan(DateTime.Now.AddYears(-18)).WithMessage("Employee must be over 18 years of age");
        }
    }
}
