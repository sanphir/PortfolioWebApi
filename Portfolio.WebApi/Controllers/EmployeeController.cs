using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;
using Portfolio.WebApi.Mappers;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly DemoAppDbContext _context;
        private readonly IEmployeeMapper _employeeMapper;
        private readonly IValidator<Employee> _validator;

        public EmployeeController(DemoAppDbContext context, IValidator<Employee> validator, IEmployeeMapper employeeMapper)
        {
            _context = context;
            _employeeMapper = employeeMapper;
            _validator = validator;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> Get()
        {
            //Loader testing
            //await Task.Delay(3000);

            var result = (await _context.Employees.ToListAsync()).Select(e => _employeeMapper.MapEmployeeDTO(e));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> Get(Guid id)
        {
            //await Task.Delay(3000);
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                return Ok(_employeeMapper.MapEmployeeDTO(employee));
            }
            else
            {
                return BadRequest($"Employee with id=\"{id}\"  not  found");
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPost("add")]
        public async Task<ActionResult<EmployeeDTO>> AddEmployee(NewEmployeeDTO newEmployee)
        {
            var existingEmployee = _context.Employees.FirstOrDefault(r => r.Name == newEmployee.Name);
            if (existingEmployee == null)
            {
                var employee = _employeeMapper.MapNewEmployee(newEmployee);
                var validateResult = _validator.Validate(employee);
                if (!validateResult.IsValid)
                {
                    var messages = string.Join("; ", validateResult.Errors.Select(e => $"{e.PropertyName}: \"{e.ErrorMessage}\"").ToArray());
                    return BadRequest($"Employee validation error: {messages}");
                }
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                return Ok(_employeeMapper.MapEmployeeDTO(employee));
            }
            else
            {
                return BadRequest($"Employee \"{newEmployee.Name}\" already exists");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<EmployeeDTO>> Update(UpdateEmployeeDTO employeeDTO)
        {
            var employee = await _context.Employees.FindAsync(employeeDTO.Id);
            if (employee != null)
            {
                employee.Email = employeeDTO.Email;
                employee.BirthDate = employeeDTO.BirthDate;
                employee.Salary = employeeDTO.Salary;
                employee.Role = employeeDTO.Role;

                var validateResult = _validator.Validate(employee);
                if (!validateResult.IsValid)
                {
                    var messages = string.Join("; ", validateResult.Errors.Select(e => $"{e.PropertyName}: \"{e.ErrorMessage}\"").ToArray());
                    return BadRequest($"Employee validation error: {messages}");
                }

                await _context.SaveChangesAsync();
                return Ok(_employeeMapper.MapEmployeeDTO(employee));
            }
            else
            {
                return BadRequest($"Employee with id=\"{employeeDTO.Id}\"  not  found");
            }

        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        public async Task<ActionResult> RemoveEmployee(IEnumerable<Guid> idsToRemove)
        {
            try
            {
                var employees = _context.Employees.Where(r => idsToRemove.Contains(r.Id));
                if (employees != null)
                {
                    _context.Employees.RemoveRange(employees);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
