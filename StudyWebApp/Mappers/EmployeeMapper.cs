using StudyProj.DAL.Models;
using StudyProj.WebApp.DTO;
using StudyProj.WebApp.Security;

namespace StudyProj.WebApp.Mappers
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
                Role = employee.Role,
                CreatedDate = employee.CreatedDate,
                LastModifiedDate = employee.LastModifiedDate
            };
        }

        public Employee MapNewEmployee(NewEmployeeDTO newEmployee)
        {
            return new Employee
            {
                Name = newEmployee.Name,
                Email = newEmployee.Email,
                Password = _passwordHasher.Hash(newEmployee.Password),
                Role = newEmployee.Role,
                BirthDate = newEmployee.BirthDate,
                Salary = newEmployee.Salary
            };
        }
    }
}
