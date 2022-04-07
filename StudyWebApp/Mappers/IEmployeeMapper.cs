using StudyProj.DAL.Models;
using StudyProj.WebApp.DTO;

namespace StudyProj.WebApp.Mappers
{
    public interface IEmployeeMapper
    {
        EmployeeDTO MapEmployeeDTO(Employee employee);

        Employee MapNewEmployee(NewEmployeeDTO newEmployee);
    }
}