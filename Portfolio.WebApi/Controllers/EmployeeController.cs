using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;
using Portfolio.WebApi.Mappers;
using System.Text.Json;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly DemoAppDbContext _context;
        private readonly IEmployeeMapper _employeeMapper;
        private readonly IValidator<Employee> _validator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(DemoAppDbContext context, IValidator<Employee> validator, IEmployeeMapper employeeMapper, IConfiguration configuration, ILogger<EmployeeController> logger)
        {
            _context = context;
            _employeeMapper = employeeMapper;
            _validator = validator;
            _configuration = configuration;
            _logger = logger;
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
                _logger.LogWarning("{requestMethod}:{requestPath}: Employee with id=\"{id}\"  not  found", Request.Method, Request.Path, id.ToString());
                return BadRequest($"Employee with id=\"{id}\"  not  found");
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPost("add")]
        public async Task<ActionResult<EmployeeDTO>> Add(NewEmployeeDTO newEmployee)
        {
            if (int.TryParse(_configuration.GetSection("Options:EmployeesLimit")?.Value ?? "100", out int employeesLimit))
            {
                if (_context.Employees.Count() >= employeesLimit)
                {
                    _logger.LogWarning("{requestMethod}:{requestPath}: Was reached the limit over {employeesLimit} employees", Request.Method, Request.Path, JsonSerializer.Serialize(employeesLimit));
                    return BadRequest($"You have reached the limit over {employeesLimit} employees");
                }
            }

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

                _logger.LogInformation("{requestMethod}:{requestPath}: Employee was added \"{employee}\"", Request.Method, Request.Path, JsonSerializer.Serialize(employee));

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

                _logger.LogInformation("{requestMethod}:{requestPath}: Employee was updated \"{employee}\"", Request.Method, Request.Path, JsonSerializer.Serialize(employee));

                return Ok(_employeeMapper.MapEmployeeDTO(employee));
            }
            else
            {
                _logger.LogWarning("{requestMethod}:{requestPath}: Employee with id=\"{id}\" not found", Request.Method, Request.Path, employeeDTO.Id);

                return BadRequest($"Employee with id=\"{employeeDTO.Id}\" not found");
            }

        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        public async Task<ActionResult> Remove(IEnumerable<Guid> idsToRemove)
        {
            var employees = _context.Employees.Where(r => idsToRemove.Contains(r.Id));
            if (!employees.Any())
            {
                _logger.LogWarning("{requestMethod}:{requestPath}: Remove: Employee with id in {notFoundedIds} not found", Request.Method, Request.Path, JsonSerializer.Serialize(idsToRemove));
                return BadRequest($"Employee with id in {JsonSerializer.Serialize(idsToRemove)} not found");
            }

            if (employees.Count() == idsToRemove.Count())
            {
                _context.Employees.RemoveRange(employees);
                await _context.SaveChangesAsync();

                _logger.LogInformation("{requestMethod}:{requestPath}: Remove: Employees with id in {idsToRemove} was deleted", Request.Method, Request.Path, JsonSerializer.Serialize(idsToRemove));

                return Ok();
            }
            else
            {
                var employeesIds = employees.Select(e => e.Id);
                var notFoundedIds = idsToRemove.Where(r => !employeesIds.Contains(r));

                _logger.LogWarning("{requestMethod}:{requestPath}: Remove: Employee with id in {notFoundedIds} not found", Request.Method, Request.Path, JsonSerializer.Serialize(notFoundedIds));
                return BadRequest($"Employee with id in {JsonSerializer.Serialize(notFoundedIds)} not found");
            }
        }
    }
}
