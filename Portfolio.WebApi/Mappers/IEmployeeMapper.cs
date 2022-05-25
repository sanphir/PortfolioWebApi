using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;

namespace Portfolio.WebApi.Mappers
{
    public interface IEmployeeMapper
    {
        EmployeeDTO MapEmployeeDTO(Employee employee);

        Employee MapNewEmployee(NewEmployeeDTO newEmployee);
    }
}