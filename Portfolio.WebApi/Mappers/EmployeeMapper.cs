using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;
using Portfolio.WebApi.Security;

namespace Portfolio.WebApi.Mappers
{
    public class EmployeeMapper : IEmployeeMapper
    {
        private readonly IPasswordHasher _passwordHasher;
        public EmployeeMapper(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }
        public EmployeeDTO MapEmployeeDTO(Employee employee)
        {
            return new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                BirthDate = employee.BirthDate,
                Salary = employee.Salary,
                Role = employee.Role.ToString().ToLower(),
                CreatedDate = employee.CreatedDate,
                LastModifiedDate = employee.LastModifiedDate
            };
        }

        public Employee MapNewEmployee(NewEmployeeDTO newEmployee)
        {
            var role = newEmployee.Role.ToLower() switch
            {
                "admin" => Role.Admin,
                "user" => Role.User,
                _ => Role.User
            };

            return new Employee
            {
                Name = newEmployee.Name,
                Email = newEmployee.Email,
                Password = _passwordHasher.Hash(newEmployee.Password),
                Role = role,
                BirthDate = newEmployee.BirthDate,
                Salary = newEmployee.Salary
            };
        }
    }
}
